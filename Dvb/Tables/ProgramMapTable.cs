using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SatIp.Analyzer.DVB.Descriptors;
using System.Collections;

namespace SatIp.Analyzer
{
    public enum StreamType
    {
        StreamType_11172_VIDEO = 0x01,
        StreamType_13818_VIDEO = 0x02,
        StreamType_11172_AUDIO = 0x03,
        StreamType_13818_AUDIO = 0x04,
        StreamType_13818_PRIVATE = 0x05,
        StreamType_13818_PES_PRIVATE = 0x06,
        StreamType_13522_MHPEG = 0x07,
        StreamType_13818_DSMCC = 0x08,
        StreamType_ITU_222_1 = 0x09,
        StreamType_13818_A = 0x0a,
        StreamType_13818_B = 0x0b,
        StreamType_13818_C = 0x0c,
        StreamType_13818_D = 0x0d,
        StreamType_13818_AUX = 0x0e,
    }
    public class ProgramMap
    {
        public StreamType StreamType;
        public byte Reserved;
        public ushort ElementaryPID;
        public byte Reserved2;
        public ushort ElementaryStreamInfoLength;
        public Descriptor[] Descriptors;
    }
    public class ProgramMapTable
    {
        public int TableId;
        public int SyntaxIndicator;
        public int Length;
        public int ProgramNumber;
        public int VersionNumber;
        public int CurrentNextIndicator;
        public int SectionNumber;
        public int LastSectionNumber;
        public byte Reserved3;
        public ushort PcrPID;
        public byte Reserved4;
        public ushort ProgramInfoLength;
        public Descriptor[] Descriptors;
        public ArrayList Streams; 
        public ProgramMapTable()
        {
            
        }
        public static ProgramMapTable Decode(bool start, int point, byte[] buffer)
        {
            var pmt = new ProgramMapTable(); 
            pmt.TableId = buffer[point+1];
            pmt.SyntaxIndicator = (buffer[point + 1] >> 7) & 1;
            pmt.Length = ((buffer[point + 2] & 15) * 0x100) + buffer[point + 3];
            pmt.ProgramNumber = (buffer[point + 4] * 0x100) + buffer[point + 5];
            pmt.VersionNumber = buffer[point + 8];
            pmt.CurrentNextIndicator = buffer[point + 5] & 1;
            pmt.LastSectionNumber = buffer[point + 8];
            pmt.SectionNumber = buffer[point + 7];
            pmt.Reserved3 = (byte)(buffer[point + 8] >> 5);
            pmt.PcrPID = (ushort)((buffer[point + 8] & 0x1F) << 8 | buffer[point + 9]);
            pmt.Reserved4 = (byte)(buffer[point + 10] >> 4);
            pmt.ProgramInfoLength = (byte)((buffer[point + 10] & 0x0F) << 8 | buffer[point + 11]);
            pmt.Streams = new ArrayList();
            int offset = 13;
            //if(ProgramInfoLength>0)
            //{
            //    Descriptors = Descriptor.ParseDescriptors(buffer, offset, ProgramInfoLength);
            //}
            offset += pmt.ProgramInfoLength;
            var streamLen = buffer.Length - pmt.ProgramInfoLength - 16;
            while(streamLen>=5)
            {
                ProgramMap map= new ProgramMap();
                map.StreamType = (StreamType)buffer[offset];
                map.ElementaryPID = (ushort)(((buffer[offset+1]&0x1F)<<8)+buffer[offset+2]);
                map.ElementaryStreamInfoLength =(ushort)(((buffer[offset + 3] & 0x0F) << 8) + buffer[offset + 4]);
                map.Descriptors = Descriptor.ParseDescriptors(buffer, offset + 5, map.ElementaryStreamInfoLength);
                pmt.Streams.Add(map);
            
                offset += map.ElementaryStreamInfoLength + 5;
                streamLen -= map.ElementaryStreamInfoLength + 5;
            }
            return pmt;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Program Map Table.\n");
            sb.AppendFormat("Table ID : {0} .\n", TableId);
            sb.AppendFormat("Syntax Indicator : {0} .\n", SyntaxIndicator);
            sb.AppendFormat("Length : {0} .\n", Length);
            sb.AppendFormat("Program Number: {0} .\n", ProgramNumber);
            sb.AppendFormat("Version Number : {0} .\n", VersionNumber);
            sb.AppendFormat("Current / Next Indicator : {0} .\n", CurrentNextIndicator);
            sb.AppendFormat("Section Number : {0} .\n", SectionNumber);
            sb.AppendFormat("Last Section Number : {0} .\n", LastSectionNumber);
            //TODO 
            // Read Descriptos
            // Read Elementary Streams
            sb.AppendFormat(".\n");
            return sb.ToString();
        }
    }
}
