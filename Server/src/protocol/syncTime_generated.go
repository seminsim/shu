// automatically generated by the FlatBuffers compiler, do not modify

package protocol

import (
	flatbuffers "github.com/google/flatbuffers/go"
)

type SyncTime struct {
	_tab flatbuffers.Table
}

func GetRootAsSyncTime(buf []byte, offset flatbuffers.UOffsetT) *SyncTime {
	n := flatbuffers.GetUOffsetT(buf[offset:])
	x := &SyncTime{}
	x.Init(buf, n+offset)
	return x
}

func (rcv *SyncTime) Init(buf []byte, i flatbuffers.UOffsetT) {
	rcv._tab.Bytes = buf
	rcv._tab.Pos = i
}

func (rcv *SyncTime) Table() flatbuffers.Table {
	return rcv._tab
}

func (rcv *SyncTime) ServerTime() int64 {
	o := flatbuffers.UOffsetT(rcv._tab.Offset(4))
	if o != 0 {
		return rcv._tab.GetInt64(o + rcv._tab.Pos)
	}
	return 0
}

func (rcv *SyncTime) MutateServerTime(n int64) bool {
	return rcv._tab.MutateInt64Slot(4, n)
}

func SyncTimeStart(builder *flatbuffers.Builder) {
	builder.StartObject(1)
}
func SyncTimeAddServerTime(builder *flatbuffers.Builder, serverTime int64) {
	builder.PrependInt64Slot(0, serverTime, 0)
}
func SyncTimeEnd(builder *flatbuffers.Builder) flatbuffers.UOffsetT {
	return builder.EndObject()
}
