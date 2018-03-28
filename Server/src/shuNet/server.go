package shuNet

import (
	"net"
	"strconv"
	"sync"
)

type handler interface {
	onConn(*Socket)
	onRecv(*Socket, []byte) error
	onDisc(*Socket, error)
	makePacket([]byte) []byte
}

type Server struct {
	conns    *sync.Map
	handler  handler
	listener net.Listener
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
	s.listener = l
	go handleListen(s)
	return nil
}

func (s *Server) Close() error {
	s.conns.Range(func(key, value interface{}) bool {
		key.(*Socket).Close()
		return true
	})
	return s.listener.Close()
}

func (s *Server) Broadcast(data []byte) {
	s.conns.Range(func(key, value interface{}) bool {
		key.(*Socket).Write(data)
		return true
	})
}

func handleListen(s *Server) {
	for {
		// Listen for an incoming connection.
		conn, err := s.listener.Accept()
		if err != nil {
			return
		}
		// Handle connections in a new goroutine.
		socket := newSocket(s, conn, s.handler)
		s.conns.Store(socket, true)
		socket.Start()
	}
}

func (s *Server) onDisc(socket *Socket, err error) {
	s.conns.Delete(socket)
	s.handler.onDisc(socket, err)
}
