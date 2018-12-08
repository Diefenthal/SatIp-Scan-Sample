using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp.Ts
{
    class TsPacket
    {
        private TsHeader _header;

        public TsPacket()
        {
        }
        public static TsPacket Decode(byte[] buffer)
        {
            var packet = new TsPacket();
            packet._header = TsHeader.Decode(buffer);
            return packet;
        }
        public TsHeader Header { get => _header; set => _header = value; }
    }
}
