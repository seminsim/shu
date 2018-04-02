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
	client := shuNet.NewClient(onConn, onRecv, onDisc)
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
