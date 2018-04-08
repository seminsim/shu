using System;
using UnityEngine;

namespace SHU {
  namespace Network {
    public class ServerTime : MonoBehaviour {
      long serverTime;
      long diffTime;
      float rttTime;
      float alpha = 0.5f;
      const long MilliToNano = 1000000;
      private System.Object thisLock = new System.Object();

      public void OnRecvTimePacket(PacketData data) {
        lock (thisLock) {
          if (data.packetID == PacketID.SyncTime) {
            serverTime = (long)(BitConverter.ToUInt64 (data.data, 0));
            diffTime = GetNowUnixTime () - serverTime;
          } else if (data.packetID == PacketID.Pong) {
            long clientTime = (long)(BitConverter.ToUInt64 (data.data, 0));
            serverTime = (long)(BitConverter.ToUInt64 (data.data, 8));

            long currentTime = GetNowUnixTime ();
            diffTime = currentTime - serverTime;

            if (rttTime == 0) {
              rttTime = (float)((currentTime - clientTime)/MilliToNano);
            } else {
              rttTime = alpha * rttTime + (1 - alpha) * (float)((currentTime - clientTime)/MilliToNano);
            }

            Debug.Log (string.Format ("Ping RTT={0}, Diff={1}", rttTime, diffTime));
          }
        }
      }

      public static long GetNowUnixTime() {
        return (long)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1)).TotalMilliseconds * MilliToNano);
      }

      public long GetServerTime(long clientTime) {
        return clientTime - diffTime + (long)(rttTime / 2 * MilliToNano);
      }

      public long GetClientTime(long serverTime) {
        return serverTime + diffTime - (long)(rttTime / 2 * MilliToNano);
      }
    }
  }
}

