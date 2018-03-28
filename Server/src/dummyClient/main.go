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

func main() {
	client := shuNet.NewClient(shuNet.NewPacketHandler(onConn, onRecv, onDisc))
	client.Dial(CONN_HOST, CONN_PORT)

	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		str := scanner.Text()
		if str == "exit" {
			break
		}
		client.Write([]byte(str))
	}

	client.Close()
}

func onConn(socket *shuNet.Socket) {
	fmt.Println("OnConnect ", socket)
}

func onRecv(socket *shuNet.Socket, data []byte) error {
	fmt.Println("OnRecv size:", len(data), " ", string(data))
	return nil
}

func onDisc(socket *shuNet.Socket, err error) {
	fmt.Println("OnDisconnect ", err)
}
