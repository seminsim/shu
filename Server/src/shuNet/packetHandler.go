package shuNet

import (
	"encoding/binary"
)

const (
	read_size = 0
	read_data = 1
)

// PacketHandler is a Shu normal packetHandler.
type PacketHandler struct {
	_onConn func(*Socket)
	_onRecv func(*Socket, []byte) error
	_onDisc func(*Socket, error)

	process int
	sizeBuf []byte
	size    uint16
	data    []byte
	readPos int
}

// NewPacketHandler makes new PacketHAndler
func NewPacketHandler(onConn func(*Socket),
	onRecv func(*Socket, []byte) error,
	onDisc func(*Socket, error)) *PacketHandler {
	handler := &PacketHandler{}
	handler._onConn = onConn
	handler._onRecv = onRecv
	handler._onDisc = onDisc
	handler.sizeBuf = make([]byte, 2)
	handler.process = read_size
	return handler
}

func (p *PacketHandler) onConn(socket *Socket) {
	p._onConn(socket)
}

type errorString struct {
	s string
}

func (e *errorString) Error() string {
	return e.s
}

func (p *PacketHandler) onRecv(socket *Socket, data []byte) error {
	idx := 0
	for idx < len(data) {
		if p.process == read_size {
			for ; idx < len(data) && p.readPos < 2; idx++ {
				p.sizeBuf[p.readPos] = data[idx]
				p.readPos++
			}
			if p.readPos == 2 {
				p.size = binary.LittleEndian.Uint16(p.sizeBuf)
				if p.size == 0 {
					return &errorString{"size could be not zero."}
				}
				p.process = read_data
				p.readPos = 0
				p.data = make([]byte, p.size)
			}
		} else {
			for ; idx < len(data) && p.readPos < int(p.size); idx++ {
				p.data[p.readPos] = data[idx]
				p.readPos++
			}
			if p.readPos == int(p.size) {
				p._onRecv(socket, p.data)
				p.process = read_size
				p.readPos = 0
			}
		}
	}

	return nil
}

func (p *PacketHandler) onDisc(socket *Socket, err error) {
	p._onDisc(socket, err)
}

func (p *PacketHandler) makePacket(buf []byte) []byte {
	newBuf := make([]byte, 2+len(buf))
	binary.LittleEndian.PutUint16(newBuf, uint16(len(buf)))
	for i, d := range buf {
		newBuf[i+2] = d
	}
	return newBuf
}
