package main

import (
	"bufio"
	"fmt"
	"os"
	"protocol"
	"shuNet"
	"time"

	flatbuffers "github.com/google/flatbuffers/go"
)

const (
	CONN_HOST = "0.0.0.0"
	CONN_PORT = 3333
)

var serverTime *ServerTime
var client *shuNet.Client
var builder *flatbuffers.Builder

func main() {
	builder = flatbuffers.NewBuilder(0)
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
	buf := MakePing(builder, time.Now().UnixNano())
	client.Write(&shuNet.PacketInfo{PacketID: protocol.PacketIDPing, Data: buf})
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(*shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data))

	if pkt.PacketID == protocol.PacketIDSyncTime {
		syncTime := protocol.GetRootAsSyncTime(pkt.Data, 0)
		st := syncTime.ServerTime()
		serverTime.OnRecvSyncTime(st)

	} else if pkt.PacketID == protocol.PacketIDPong {
		pong := protocol.GetRootAsPong(pkt.Data, 0)
		ct := pong.ClientTime()
		st := pong.ServerTime()

		serverTime.OnRecvPong(ct, st)
		fmt.Println("Ping RTT=", serverTime.rttTime/float64(time.Millisecond.Nanoseconds()), " Diff=", serverTime.diffTime)
	}

	return nil
}

func MakePing(b *flatbuffers.Builder, clientTime int64) []byte {
	// re-use the already-allocated Builder:
	b.Reset()

	protocol.PingStart(b)
	protocol.PingAddClientTime(b, clientTime)
	ping := protocol.PingEnd(b)
	b.Finish(ping)

	// return the byte slice containing encoded data:
	return b.FinishedBytes()
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
