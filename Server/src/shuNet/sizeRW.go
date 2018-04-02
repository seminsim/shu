package shuNet

import "encoding/binary"

type readHandler interface {
	onRecv(*Socket, []byte) error
}

type rwHandler interface {
	onRecv(*Socket, []byte) error
	write([]byte) []byte
}

const (
	readSize = 0
	readData = 1
)

type SizeRW struct {
	readHandler readHandler

	process int
	sizeBuf []byte
	size    uint16
	data    []byte
	readPos int
}

func NewSizeRW(readHandler readHandler) *SizeRW {
	rw := &SizeRW{readHandler: readHandler}
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

func (p *SizeRW) onRecv(socket *Socket, data []byte) error {
	idx := 0
	for idx < len(data) {
		if p.process == readSize {
			for ; idx < len(data) && p.readPos < 2; idx++ {
				p.sizeBuf[p.readPos] = data[idx]
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
			for ; idx < len(data) && p.readPos < int(p.size); idx++ {
				p.data[p.readPos] = data[idx]
				p.readPos++
			}
			if p.readPos == int(p.size) {
				p.readHandler.onRecv(socket, p.data)
				p.process = readSize
				p.readPos = 0
			}
		}
	}

	return nil
}

func (p *SizeRW) write(buf []byte) []byte {
	newBuf := make([]byte, 2+len(buf))
	binary.LittleEndian.PutUint16(newBuf, uint16(len(buf)))
	for i, d := range buf {
		newBuf[i+2] = d
	}
	return newBuf
}
