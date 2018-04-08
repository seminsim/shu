package shuNet

import "encoding/binary"

type ReadHandler interface {
	onRecv(*Socket, interface{}) error
}

type RWHandler interface {
	onRecv(*Socket, interface{}) error
	write(interface{}) interface{}
}

type readFunc func(*Socket, interface{}) error

func (f readFunc) onRecv(socket *Socket, data interface{}) error {
	return f(socket, data)
}

const (
	readSize = 0
	readData = 1
)

type SizeRW struct {
	rwHandler RWHandler

	process int
	sizeBuf []byte
	size    uint16
	data    []byte
	readPos int
}

func NewSizeRW(rwHandler RWHandler) *SizeRW {
	rw := &SizeRW{rwHandler: rwHandler}
	rw.sizeBuf = make([]byte, 2)
	rw.process = readSize
	return rw
}

type errorString struct {
	s string
}

func (e *errorString) Error() string {
	return e.s
}

func (p *SizeRW) onRecv(socket *Socket, data interface{}) error {
	buf := data.([]byte)
	idx := 0
	for idx < len(buf) {
		if p.process == readSize {
			for ; idx < len(buf) && p.readPos < 2; idx++ {
				p.sizeBuf[p.readPos] = buf[idx]
				p.readPos++
			}
			if p.readPos == 2 {
				p.size = binary.LittleEndian.Uint16(p.sizeBuf)
				if p.size == 0 {
					return &errorString{"size could be not zero."}
				}
				p.process = readData
				p.readPos = 0
				p.data = make([]byte, p.size)
			}
		} else {
			for ; idx < len(buf) && p.readPos < int(p.size); idx++ {
				p.data[p.readPos] = buf[idx]
				p.readPos++
			}
			if p.readPos == int(p.size) {
				p.rwHandler.onRecv(socket, p.data)
				p.process = readSize
				p.readPos = 0
			}
		}
	}

	return nil
}

func (p *SizeRW) write(data interface{}) interface{} {
	buf := p.rwHandler.write(data).([]byte)
	newBuf := make([]byte, 2+len(buf))
	binary.LittleEndian.PutUint16(newBuf, uint16(len(buf)))
	for i, d := range buf {
		newBuf[i+2] = d
	}
	return newBuf
}
