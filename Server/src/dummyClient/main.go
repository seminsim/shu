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

var serverTime *ServerTime
var client *shuNet.Client

func main() {
	serverTime = &ServerTime{}

	client = shuNet.NewClient(onConn, onDisc, shuNet.NewSizeRW(shuNet.NewPacketRW(onRecv)))
	err := client.Dial(CONN_HOST, CONN_PORT)
	if err != nil {
		fmt.Println(err)
		return
	}

	go pingHandler()

	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		str := scanner.Text()
		if str == "exit" {
			break
		}
		client.Write(&shuNet.PacketInfo{PacketID: 1, Data: []byte(str)})
	}

	client.Close()
}

func onConn(socket *shuNet.Socket) {
	fmt.Println("OnConnect ", socket)
}

func pingHandler() {
	fmt.Println("pingHandler")
	for {
		select {
		case <-time.After(1 * time.Second):
			sendPing()
		}
	}
}

func sendPing() {
	fmt.Println("sendPing")
	buf := MakePing(time.Now().UnixNano())
	client.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDPing, Data: buf})
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(*shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data))

	if pkt.PacketID == protocol.PacketIDSyncTime {
		st := int64(binary.LittleEndian.Uint64(pkt.Data))
		serverTime.OnRecvSyncTime(st)

	} else if pkt.PacketID == protocol.PacketIDPong {
		ct := int64(binary.LittleEndian.Uint64(pkt.Data))
		st := int64(binary.LittleEndian.Uint64(pkt.Data[8:]))

		serverTime.OnRecvPong(ct, st)
		fmt.Println("Ping RTT=", serverTime.rttTime, " Diff=", serverTime.diffTime)
	}

	return nil
}

func MakePing(clientTime int64) []byte {
	buf := make([]byte, 8)
	binary.LittleEndian.PutUint64(buf, uint64(clientTime))
	return buf
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
