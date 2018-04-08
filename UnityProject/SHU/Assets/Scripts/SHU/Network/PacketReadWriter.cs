using System;
using UnityEngine;

namespace SHU {
  namespace Network { 
    public class PacketData {
      public UInt16 packetID;
      public byte[] data;

      public PacketData (UInt16 packetID, byte[] data) {
        this.packetID = packetID;
        this.data = data;
      }
    }

    public class PacketReadWriter : ReadWriter {

      Action<PacketData> onRecv;

      public PacketReadWriter (Action<PacketData> onRecv) {
        this.onRecv = onRecv;
      }
        
      public void Read(object data) {
        var buf = data as BufData;
        Debug.Assert (buf.length >= 2);
        UInt16 packetID = BitConverter.ToUInt16 (buf.buf, 0);
        byte[] packetData = null;
        if (buf.length > 2) {
          packetData = new byte[buf.length - 2];
          Array.Copy(buf.buf, 2, packetData, 0, buf.length - 2);
        }

        onRecv (new PacketData (packetID, packetData));
      }

      public object Write(object data) {
        PacketData packet = data as PacketData;
        byte[] newBuf = new byte[2 + packet.data.Length];
        Array.Copy(BitConverter.GetBytes(packet.packetID), 0, newBuf, 0, 2);
        Array.Copy(packet.data, 0, newBuf, 2, packet.data.Length);
        return newBuf;
      }
    }
  }
}

