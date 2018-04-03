package main

import (
	"bufio"
	"fmt"
	"os"
	"protocol"
	"shuNet"
	"time"

	"github.com/google/flatbuffers/go"
)

const (
	CONN_HOST = "0.0.0.0"
	CONN_PORT = 3333
)

var server *shuNet.Server
var builder *flatbuffers.Builder

func main() {
	server = shuNet.NewServer(onConn, onDisc, shuNet.NewPacketRWCB(onRecv))
	builder = flatbuffers.NewBuilder(0)
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

	buf := MakeSyncTime(builder, time.Now().UnixNano())
	socket.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDSyncTime, Data: buf})
}

func MakeSyncTime(b *flatbuffers.Builder, serverTime int64) []byte {
	// re-use the already-allocated Builder:
	b.Reset()

	protocol.SyncTimeStart(b)
	protocol.SyncTimeAddServerTime(b, serverTime)
	sync := protocol.SyncTimeEnd(b)

	b.Finish(sync)

	// return the byte slice containing encoded data:
	return b.FinishedBytes()
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(*shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data))

	if pkt.PacketID == protocol.PacketIDPing {
		ping := protocol.GetRootAsPing(pkt.Data, 0)
		clientTime := ping.ClientTime()

		buf := MakePong(builder, clientTime, time.Now().UnixNano())
		socket.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDPong, Data: buf})
	} else {
		server.Broadcast(pkt)
	}
	return nil
}

func MakePong(b *flatbuffers.Builder, clientTime int64, serverTime int64) []byte {
	// re-use the already-allocated Builder:
	b.Reset()

	protocol.PongStart(b)
	protocol.PongAddClientTime(b, clientTime)
	protocol.PongAddServerTime(b, serverTime)
	pong := protocol.PongEnd(b)

	b.Finish(pong)

	// return the byte slice containing encoded data:
	return b.FinishedBytes()
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
