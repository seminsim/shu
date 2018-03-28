package main

import (
	"bufio"
	"fmt"
	"os"
	"shuNet"
)

const (
	CONN_HOST = "localhost"
	CONN_PORT = 3333
)

var server *shuNet.Server

func main() {
	server = shuNet.NewServer(shuNet.NewPacketHandler(onConn, onRecv, onDisc))
	server.Start(CONN_HOST, CONN_PORT)

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

func onRecv(socket *shuNet.Socket, data []byte) error {
	fmt.Println("OnRecv size:", len(data), " ", string(data))
	server.Broadcast(data)
	return nil
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
