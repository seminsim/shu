package main

import (
	"bufio"
	"fmt"
	"os"
	"shuNet"
)

const (
	CONN_HOST = "0.0.0.0"
	CONN_PORT = 3333
)

var server *shuNet.Server

func main() {
	server = shuNet.NewServer(onConn, onDisc, shuNet.NewPacketRWCB(onRecv))
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
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data), " ", string(pkt.Data))

	server.Broadcast(pkt)
	return nil
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
