package shuNet

import (
	"net"
	"strconv"
	"sync"
)

type Server struct {
	conns     *sync.Map
	RWHandler rwHandler
	listener  net.Listener

	connCallback func(*Socket)
	recvCallback func(*Socket, []byte) error
	discCallback func(*Socket, error)
}

func NewServer(onConn func(*Socket),
	onRecv func(*Socket, []byte) error,
	onDisc func(*Socket, error)) *Server {
	server := &Server{}
	server.conns = new(sync.Map)

	server.connCallback = onConn
	server.recvCallback = onRecv
	server.discCallback = onDisc

	server.RWHandler = NewSizeRW(server)
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
	buf := s.RWHandler.write(data)
	s.conns.Range(func(key, value interface{}) bool {
		key.(*Socket).write(buf)
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
		socket := newSocket(s, conn, s.RWHandler)
		s.conns.Store(socket, true)

		s.connCallback(socket)
		socket.Start()
	}
}

func (s *Server) onRecv(socket *Socket, data []byte) error {
	return s.recvCallback(socket, data)
}

func (s *Server) onDisc(socket *Socket, err error) {
	s.conns.Delete(socket)
	s.discCallback(socket, err)
}
