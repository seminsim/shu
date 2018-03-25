package shuNet

import (
	"net"
	"strconv"
	"sync"
)

type handler interface {
	onConn(net.Conn)
	onRecv(net.Conn, []byte) error
	onDisc(net.Conn, error)
}

type Server struct {
	conns   *sync.Map
	handler handler
}

func NewServer(handler handler) *Server {
	server := &Server{}
	server.conns = new(sync.Map)
	server.handler = handler
	return server
}

func (s *Server) Start(host string, port uint16) error {
	l, err := net.Listen("tcp", host+":"+strconv.Itoa(int(port)))
	if err != nil {
		return err
	}
	defer l.Close()
	for {
		// Listen for an incoming connection.
		conn, err := l.Accept()
		if err != nil {
			return err
		}
		// Handle connections in a new goroutine.
		go handleRequest(s, conn)
	}
}

func handleRequest(s *Server, conn net.Conn) {
	s.conns.Store(conn, true)
	s.handler.onConn(conn)

	// Make a buffer to hold incoming data.
	buf := make([]byte, 1024)
	for {
		// Read the incoming connection into the buffer.
		readLen, err := conn.Read(buf)
		if err != nil {
			s.conns.Delete(conn)
			s.handler.onDisc(conn, err)
			return
		}
		if err = s.handler.onRecv(conn, buf[:readLen]); err != nil {
			s.conns.Delete(conn)
			s.handler.onDisc(conn, err)
			return
		}
	}
}
