using System;

namespace SHU {
  namespace Network {
    public class BufData {
      public int length;
      public byte[] buf;

      public BufData(int length, byte[] buf) {
        this.length = length;
        this.buf = buf;
      }
    }

    public interface ReadWriter {
      void Read(object data);
      object Write(object data);
    }
  }
}