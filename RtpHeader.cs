using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp.Analyzer
{
    public class RtpHeader
    {
        private static int MinHeaderLength = 12;
        public Int32 HeaderSize = MinHeaderLength;
        public Int32 Version { get; set; }
        public Boolean Padding { get; set; }
        public Boolean Extension { get; set; }
        public Int32 CsrcCount { get; set; }
        public Boolean Marker { get; set; }
        public Int32 PayloadType { get; set; }
        public UInt16 SequenceNumber { get; set; }
        public UInt32 Timestamp { get; set; }
        public UInt32 SourceId { get; set; }
        public UInt16 ExtensionHeaderId = 0;
        public UInt16 ExtensionLengthAsCount = 0;
        public Int32 ExtensionLengthInBytes = 0;
        public RtpHeader(byte[] buffer)
        {
            Decode(buffer);
        }
        public void Decode(byte[] buffer)
        {
            if (buffer.Length >= MinHeaderLength)
            {
                Version = ValueFromByte(buffer[0], 6, 2);
                Padding = Convert.ToBoolean(ValueFromByte(buffer[0], 5, 1));
                Extension = Convert.ToBoolean(ValueFromByte(buffer[0], 4, 1));
                CsrcCount = ValueFromByte(buffer[0], 0, 4);
                Marker = Convert.ToBoolean(ValueFromByte(buffer[1], 7, 1));
                PayloadType = ValueFromByte(buffer[1], 0, 7);
                HeaderSize = MinHeaderLength + (CsrcCount * 4);
                Byte[] seqNum = new Byte[2];
                seqNum[0] = buffer[3];
                seqNum[1] = buffer[2];
                SequenceNumber = System.BitConverter.ToUInt16(seqNum, 0);
                Byte[] timeStmp = new Byte[4];
                timeStmp[0] = buffer[7];
                timeStmp[1] = buffer[6];
                timeStmp[2] = buffer[5];
                timeStmp[3] = buffer[4];
                Timestamp = System.BitConverter.ToUInt32(timeStmp, 0);
                Byte[] srcId = new Byte[4];
                srcId[0] = buffer[11];
                srcId[1] = buffer[10];
                srcId[2] = buffer[9];
                srcId[3] = buffer[8];
                SourceId = System.BitConverter.ToUInt32(srcId, 0);
                Byte[] csrcid = new byte[4];
                if (Extension)
                {
                    Byte[] extHeaderId = new Byte[2];
                    extHeaderId[1] = buffer[HeaderSize + 0];
                    extHeaderId[0] = buffer[HeaderSize + 1];
                    ExtensionHeaderId = System.BitConverter.ToUInt16(extHeaderId, 0);
                    Byte[] extHeaderLength16 = new Byte[2];
                    extHeaderLength16[1] = buffer[HeaderSize + 2];
                    extHeaderLength16[0] = buffer[HeaderSize + 3];
                    ExtensionLengthAsCount = System.BitConverter.ToUInt16(extHeaderLength16.ToArray(), 0);
                    ExtensionLengthInBytes = ExtensionLengthAsCount * 4;
                    HeaderSize += ExtensionLengthInBytes + 4;
                }
                
            }
        }

        /// <summary>
        /// GetValueFromByte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Int32 ValueFromByte(Byte value, int startPos, int length)
        {
            Byte mask = 0;
            for (int i = 0; i < length; i++)
            {
                mask = (Byte)(mask | 0x1 << startPos + i);
            }
            Byte result = (Byte)((value & mask) >> startPos);
            return Convert.ToInt32(result);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("RTP Header");
            sb.AppendFormat("Version: {0} .\n", Version);
            sb.AppendFormat("Padding: {0} .\n", Padding);
            sb.AppendFormat("Extension: {0} .\n", Extension);
            sb.AppendFormat("Contributing Source Identifiers Count: {0} .\n", CsrcCount);
            sb.AppendFormat("Marker: {0} .\n", Marker);
            sb.AppendFormat("Payload Type: {0} .\n", PayloadType);
            sb.AppendFormat("Sequence Number: {0} .\n", SequenceNumber);
            sb.AppendFormat("Timestamp: {0} .\n", Timestamp);
            sb.AppendFormat("Synchronization Source Identifier: {0} .\n", SourceId);
            sb.AppendFormat(".\n");
            return sb.ToString();
        }

    }
}
