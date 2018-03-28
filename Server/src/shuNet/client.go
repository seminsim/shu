package shuNet

import (
	"net"
	"strconv"
)

// Client is client handler for Networking
type Client struct {
	socket  *Socket
	handler handler
}

// NewClient makes new client instance
func NewClient(handler handler) *Client {
	client := &Client{}
	client.handler = handler
	return client
}

func (c *Client) Dial(host string, port uint16) error {
	conn, err := net.Dial("tcp", host+":"+strconv.Itoa(int(port)))
	if err != nil {
		return err
	}
	c.socket = newSocket(c, conn, c.handler)
	c.socket.Start()
	return nil
}

func (c *Client) onDisc(socket *Socket, err error) {
	c.socket = nil
	c.handler.onDisc(socket, err)
}

func (c *Client) Close() error {
	return c.socket.Close()
}

func (c *Client) Write(buf []byte) {
	c.socket.Write(buf)
}
