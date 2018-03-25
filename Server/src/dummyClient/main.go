package main

import (
	"bufio"
	"fmt"
	"net"
	"os"
)

const (
	CONN_HOST = "localhost"
	CONN_PORT = "3333"
	CONN_TYPE = "tcp"
)

func main() {
	conn, err := net.Dial(CONN_TYPE, CONN_HOST+":"+CONN_PORT)
	if err != nil {
		fmt.Println("Error listening:", err.Error())
		os.Exit(1)
	}

	defer conn.Close()
	fmt.Println("Connected on " + CONN_HOST + ":" + CONN_PORT)

	go handleRequest(conn)

	scanner := bufio.NewScanner(os.Stdin)
	for scanner.Scan() {
		str := scanner.Text()
		fmt.Println(str)
		conn.Write([]byte(str))
	}
}

// Handles incoming requests.
func handleRequest(conn net.Conn) {

	buf := make([]byte, 1024)

	for {
		// Read the incoming connection into the buffer.
		_, err := conn.Read(buf)
		if err != nil {
			fmt.Println("Error reading:", err.Error())
			return
		}

		fmt.Println(string(buf))
	}
}
