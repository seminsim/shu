package shuNet

import (
	"net"
	"strconv"
	"sync"
)

// Client is client handler for Networking
type Client struct {
	socket    *Socket
	rwHandler RWHandler
	mutex     *sync.Mutex

	connCallback func(*Socket)
	discCallback func(*Socket, error)
}

// NewClient makes new client instance
func NewClient(onConn func(*Socket),
	onDisc func(*Socket, error),
	rwHandler RWHandler) *Client {
	client := &Client{rwHandler: rwHandler}
	client.mutex = &sync.Mutex{}

	client.connCallback = onConn
	client.discCallback = onDisc
	return client
}

func (c *Client) Dial(host string, port uint16) error {
	conn, err := net.Dial("tcp", host+":"+strconv.Itoa(int(port)))
	if err != nil {
		return err
	}
	c.mutex.Lock()
	defer c.mutex.Unlock()

	c.socket = newSocket(c, conn, c.rwHandler)
	c.connCallback(c.socket)
	c.socket.Start()
	return nil
}

func (c *Client) onDisc(socket *Socket, err error) {
	c.mutex.Lock()
	defer c.mutex.Unlock()

	c.socket = nil
	c.discCallback(socket, err)
}

func (c *Client) Close() error {
	c.mutex.Lock()
	defer c.mutex.Unlock()

	if c.socket != nil {
		return c.socket.Close()
	}
	return nil
}

func (c *Client) Write(data interface{}) {
	c.mutex.Lock()
	defer c.mutex.Unlock()

	if c.socket != nil {
		c.socket.Write(data)
	}
}
