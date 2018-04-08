using System;
using System.Text;
using UnityEngine;


namespace SHU {
  namespace Network {
    public class SizeReadWriter : ReadWriter {
      public SizeReadWriter (ReadWriter readWriter) {
        this.readWriter = readWriter;
      }

      ReadWriter readWriter;
      bool readSize = true;
      Byte[] sizeBuf = new byte[2];
      Byte[] dataBuf;
      int readPos = 0;
      UInt16 dataSize = 0;

      public void Read(object data) {
        var buf = data as BufData;

        int idx = 0;
        while (idx < buf.length) {
          if (readSize) {
            for (; idx < buf.length && readPos < 2; idx++) {
              sizeBuf[readPos] = buf.buf[idx];
              readPos++;
            }

            if (readPos == 2) {
              if (!BitConverter.IsLittleEndian) {
                Array.Reverse (sizeBuf);
              }
              dataSize = BitConverter.ToUInt16 (sizeBuf, 0);
              dataBuf = new byte[dataSize];
              readPos = 0;
              readSize = false;
            }
          } else {
            for (; idx < buf.length && readPos < dataSize; idx++) {
              dataBuf[readPos] = buf.buf[idx];
              readPos++;
            }
            if (readPos == dataSize) {
              readWriter.Read (new BufData (dataBuf.Length, dataBuf)); 
              readSize = true;
              readPos = 0;
            }

          }
        }
      }
      public object Write(object data) {
        var buf = readWriter.Write(data) as byte[];

        byte[] newArr = new byte[2 + buf.Length];
        byte[] lenBuf = BitConverter.GetBytes ((UInt16)(buf.Length));
        if (!BitConverter.IsLittleEndian) {
          Array.Reverse (lenBuf);
        }
        for (int i = 0; i < 2; i++) {
          newArr [i] = lenBuf [i];
        }
        for (int i = 0; i < buf.Length; i++) {
          newArr [i + 2] = buf [i];
        }
        return newArr;
      }
    }
  }
}

