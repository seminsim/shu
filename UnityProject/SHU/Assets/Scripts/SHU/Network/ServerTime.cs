using System;
using UnityEngine;

namespace SHU {
  namespace Network {
    public class ServerTime : MonoBehaviour {
      long serverTime;
      long diffTime;
      float rttTime;
      float alpha = 0.5f;
      private System.Object thisLock = new System.Object();

      public void OnRecvTimePacket(PacketData data) {
        lock (thisLock) {
          if (data.packetID == PacketID.SyncTime) {
            serverTime = (long)(BitConverter.ToUInt64 (data.data, 0));
            diffTime = DateTime.Now.ToFileTimeUtc () - serverTime;
          } else if (data.packetID == PacketID.Pong) {
            long clientTime = (long)(BitConverter.ToUInt64 (data.data, 0));
            serverTime = (long)(BitConverter.ToUInt64 (data.data, 8));

            long currentTime = DateTime.Now.ToFileTimeUtc ();
            diffTime = currentTime - serverTime;

            if (rttTime == 0) {
              rttTime = (float)(currentTime - clientTime)/10000;
            } else {
              rttTime = alpha * rttTime + (1 - alpha) * (float)(currentTime - clientTime)/10000;
            }

            Debug.Log (string.Format ("Ping RTT={0}, Diff={1}", rttTime, diffTime));
          }
        }
      }


      public long GetServerTime(long clientTime) {
        return clientTime - diffTime + (long)(rttTime / 2);
      }

      public long GetClientTime(long serverTime) {
        return serverTime + diffTime - (long)(rttTime / 2);
      }
    }
  }
}

