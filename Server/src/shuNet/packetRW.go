package shuNet

import "encoding/binary"

type PacketInfo struct {
	PacketID uint16
	Data     []byte
}

type PacketRW struct {
	readHandler ReadHandler
}

func NewPacketRW(onrecv func(*Socket, interface{}) error) *PacketRW {
	rw := &PacketRW{readHandler: readFunc(onrecv)}
	return rw
}

func (p *PacketRW) onRecv(socket *Socket, data interface{}) error {
	buf := data.([]byte)
	if len(buf) < 2 {
		return &errorString{"packet size should be more 2 bytes."}
	}
	packetID := binary.LittleEndian.Uint16(buf[:2])
	return p.readHandler.onRecv(socket, &PacketInfo{PacketID: packetID, Data: buf[2:]})
}

func (p *PacketRW) write(data interface{}) interface{} {
	pkt := data.(*PacketInfo)
	newBuf := make([]byte, len(pkt.Data)+2)
	binary.LittleEndian.PutUint16(newBuf, pkt.PacketID)
	for i, d := range pkt.Data {
		newBuf[i+2] = d
	}
	return newBuf
}
