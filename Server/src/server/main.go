package main

import (
	"bufio"
	"encoding/binary"
	"fmt"
	"os"
	"protocol"
	"shuNet"
	"time"
)

const (
	CONN_HOST = "0.0.0.0"
	CONN_PORT = 3333
)

var server *shuNet.Server

func main() {
	//shuNet.SocketWriteWaitTime = 1 * time.Millisecond
	server = shuNet.NewServer(onConn, onDisc, shuNet.NewSizeRW(shuNet.NewPacketRW(onRecv)))
	err := server.Start(CONN_HOST, CONN_PORT)
	if err != nil {
		fmt.Println(err)
		return
	}

	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		str := scanner.Text()
		if str == "exit" {
			break
		}
	}

	server.Close()
}

func onConn(socket *shuNet.Socket) {
	fmt.Println("OnConnect ", socket)

	buf := MakeSyncTime(time.Now().UnixNano() / 100)
	socket.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDSyncTime, Data: buf})
}

func MakeSyncTime(serverTime int64) []byte {
	buf := make([]byte, 16)
	binary.LittleEndian.PutUint64(buf, uint64(serverTime))
	return buf
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(*shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data))

	if pkt.PacketID == protocol.PacketIDPing {
		clientTime := int64(binary.LittleEndian.Uint64(pkt.Data))

		buf := MakePong(clientTime, time.Now().UnixNano())
		socket.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDPong, Data: buf})
	} else {
		server.Broadcast(pkt)
	}
	return nil
}

func MakePong(clientTime int64, serverTime int64) []byte {
	buf := make([]byte, 16)
	binary.LittleEndian.PutUint64(buf, uint64(clientTime))
	binary.LittleEndian.PutUint64(buf[8:], uint64(serverTime))

	// return the byte slice containing encoded data:
	return buf
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
