package main

import (
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

	if st.rttTime == 0 {
		st.rttTime = float64(currentTime - clientTime)
	} else {
		st.rttTime = alpha*st.rttTime + (1-alpha)*float64(currentTime-clientTime)
	}
}

func (st *ServerTime) GetServerTime(clientTime int64) int64 {
	return clientTime - st.diffTime + int64(st.rttTime/2)
}
