package shuNet

import (
	"net"
	"strconv"
)

// Client is client handler for Networking
type Client struct {
	socket    *Socket
	RWHandler rwHandler

	connCallback func(*Socket)
	recvCallback func(*Socket, []byte) error
	discCallback func(*Socket, error)
}

// NewClient makes new client instance
func NewClient(onConn func(*Socket),
	onRecv func(*Socket, []byte) error,
	onDisc func(*Socket, error)) *Client {
	client := &Client{}
	client.connCallback = onConn
	client.recvCallback = onRecv
	client.discCallback = onDisc

	client.RWHandler = NewSizeRW(client)
	return client
}

func (c *Client) Dial(host string, port uint16) error {
	conn, err := net.Dial("tcp", host+":"+strconv.Itoa(int(port)))
	if err != nil {
		return err
	}
	c.socket = newSocket(c, conn, c.RWHandler)
	c.connCallback(c.socket)
	c.socket.Start()
	return nil
}

func (c *Client) onDisc(socket *Socket, err error) {
	c.socket = nil
	c.discCallback(socket, err)
}

func (c *Client) Close() error {
	return c.socket.Close()
}

func (c *Client) Write(buf []byte) {
	c.socket.Write(buf)
}

func (c *Client) onRecv(socket *Socket, data []byte) error {
	return c.recvCallback(socket, data)
}
