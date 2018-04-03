package shuNet

import (
	"net"
	"strconv"
)

// Client is client handler for Networking
type Client struct {
	socket    *Socket
	rwHandler RWHandler

	connCallback func(*Socket)
	discCallback func(*Socket, error)
}

// NewClient makes new client instance
func NewClient(onConn func(*Socket),
	onDisc func(*Socket, error),
	rwHandler RWHandler) *Client {
	client := &Client{rwHandler: rwHandler}
	client.connCallback = onConn
	client.discCallback = onDisc
	return client
}

func (c *Client) Dial(host string, port uint16) error {
	conn, err := net.Dial("tcp", host+":"+strconv.Itoa(int(port)))
	if err != nil {
		return err
	}
	c.socket = newSocket(c, conn, c.rwHandler)
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

func (c *Client) Write(data interface{}) {
	c.socket.Write(data)
}
