package shuNet

import "encoding/binary"

type PacketInfo struct {
	PacketID uint16
	Data     []byte
}

type PacketRW struct {
	readHandler ReadHandler
	rwHandler   RWHandler
}

func NewPacketRW(readHandler ReadHandler) *PacketRW {
	rw := &PacketRW{readHandler: readHandler}
	rw.rwHandler = NewSizeRWCB(func(socket *Socket, data interface{}) error {
		return rw.internalOnRecv(socket, data)
	})
	return rw
}

func NewPacketRWCB(recvCB func(*Socket, interface{}) error) *PacketRW {
	rw := &PacketRW{readHandler: readFunc(recvCB)}
	rw.rwHandler = NewSizeRWCB(func(socket *Socket, data interface{}) error {
		return rw.internalOnRecv(socket, data)
	})
	return rw
}

func (p *PacketRW) onRecv(socket *Socket, data interface{}) error {
	return p.rwHandler.onRecv(socket, data)
}

func (p *PacketRW) internalOnRecv(socket *Socket, data interface{}) error {
	buf := data.([]byte)
	if len(buf) < 2 {
		return &errorString{"packet size should be more 2 bytes."}
	}
	packetID := binary.LittleEndian.Uint16(buf[:2])
	return p.readHandler.onRecv(socket, PacketInfo{packetID, buf[2:]})
}

func (p *PacketRW) write(data interface{}) interface{} {
	pkt := data.(PacketInfo)
	newBuf := make([]byte, len(pkt.Data)+2)
	binary.LittleEndian.PutUint16(newBuf, pkt.PacketID)
	for i, d := range pkt.Data {
		newBuf[i+2] = d
	}
	return p.rwHandler.write(newBuf)
}
