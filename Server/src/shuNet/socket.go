package shuNet

import (
	"bytes"
	"net"
	"sync/atomic"
	"time"
)

// SocketWriteWaitTime represents how long wait for sending
var SocketWriteWaitTime time.Duration = 30 * time.Millisecond

type socketOwner interface {
	onDisc(*Socket, error)
}

// Socket is shuSocket
type Socket struct {
	conn      net.Conn
	sendChan  chan []byte
	owner     socketOwner
	connected int32
	rwHandler RWHandler
}

func newSocket(owner socketOwner, conn net.Conn, rwHandler RWHandler) *Socket {
	socket := &Socket{owner: owner, conn: conn, rwHandler: rwHandler}
	socket.sendChan = make(chan []byte, 128)
	atomic.StoreInt32(&socket.connected, 1)
	return socket
}

func (s *Socket) Start() {
	go handleRead(s)
	go handleWrite(s)
}

func (s *Socket) Close() error {
	return s.conn.Close()
}

func (s *Socket) Write(data interface{}) {
	rst := s.rwHandler.write(data)
	s.sendChan <- rst.([]byte)
}

func (s *Socket) write(buf []byte) {
	s.sendChan <- buf
}

func (s *Socket) IsConnected() bool {
	return atomic.LoadInt32(&s.connected) == 1
}

func (s *Socket) onDisc(err error) {
	if atomic.CompareAndSwapInt32(&s.connected, 1, 0) {
		s.owner.onDisc(s, err)
	}
}

func handleRead(socket *Socket) {
	// Make a buffer to hold incoming data.
	buf := make([]byte, 1024)
	timeoutDuration := 5 * time.Second

	for {
		// Read the incoming connection into the buffer.
		socket.conn.SetReadDeadline(time.Now().Add(timeoutDuration))

		readLen, err := socket.conn.Read(buf)
		if err != nil {
			timeout := false
			if netErr, ok := err.(net.Error); ok {
				if netErr.Timeout() {
					timeout = true
				}
			}
			if !timeout {
				socket.onDisc(err)
				return
			}
		}
		if readLen == 0 {
			continue
		}
		if err = socket.rwHandler.onRecv(socket, buf[:readLen]); err != nil {
			socket.onDisc(err)
			return
		}
	}
}

func handleWrite(socket *Socket) {
	buf := bytes.Buffer{}
	for {
		select {
		case packet := <-socket.sendChan:
			buf.Write(packet)
		case <-time.After(SocketWriteWaitTime):
			if buf.Len() > 0 {
				writeLen, err := socket.conn.Write(buf.Bytes())
				if writeLen != buf.Len() || err != nil {
					socket.onDisc(err)
					break
				}
				buf.Reset()
			}
		}
	}
}
