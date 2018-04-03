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

func main() {
	client := shuNet.NewClient(onConn, onDisc, shuNet.NewPacketRWCB(onRecv))
	err := client.Dial(CONN_HOST, CONN_PORT)
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
		client.Write(shuNet.PacketInfo{PacketID: 1, Data: []byte(str)})
	}

	client.Close()
}

func onConn(socket *shuNet.Socket) {
	fmt.Println("OnConnect ", socket)
}

func onRecv(socket *shuNet.Socket, data interface{}) error {
	pkt := data.(shuNet.PacketInfo)
	fmt.Println("OnRecv packetID:", pkt.PacketID, " data size:", len(pkt.Data), " ", string(pkt.Data))
	return nil
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
