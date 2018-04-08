package main

import (
	"fmt"
	"time"
)

type ServerTime struct {
	serverTime int64
	diffTime   int64
	rttTime    float64
}

var alpha float64 = 0.5

func (st *ServerTime) OnRecvSyncTime(serverTime int64) {
	st.serverTime = serverTime
	st.diffTime = time.Now().UnixNano() - serverTime
}

func (st *ServerTime) OnRecvPong(clientTime int64, serverTime int64) {
	currentTime := time.Now().UnixNano()
	st.serverTime = serverTime
	st.diffTime = currentTime - serverTime

	fmt.Println("OnRecvPong currentTime:", currentTime, " clientTime:", clientTime)

	if st.rttTime == 0 {
		st.rttTime = float64(currentTime-clientTime) / float64(time.Millisecond)
	} else {
		st.rttTime = alpha*st.rttTime + (1-alpha)*(float64(currentTime-clientTime)/float64(time.Millisecond))
	}
}

func (st *ServerTime) GetServerTime(clientTime int64) int64 {
	return clientTime - st.diffTime + int64(st.rttTime/2*float64(time.Millisecond))
}

func (st *ServerTime) GetClientTime(serverTime int64) int64 {
	return serverTime + st.diffTime - int64(st.rttTime/2*float64(time.Millisecond))
}
