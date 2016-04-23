using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public class TsHeader
    {
        public byte SyncByte;
        public bool TransportErrorIndicator;
        public bool PayloadUnitStartIndicator;
        public bool TransportPriority;
        public int Pid;
        public byte TransportScramblingControl;
        public byte AdaptionFieldControl;
        public byte ContinuityCounter;
        public byte AdaptionFieldLength;
        public byte PayLoadStart;
        public bool HasAdaptionField;
        public bool HasPayload;

        public TsHeader()
        {
            TransportErrorIndicator = true;
        }
        
        public static TsHeader Decode(byte[] buffer)
        {
            var header = new TsHeader();
            header.SyncByte = buffer[0];
            if (header.SyncByte != 0x47)
            {
                header.TransportErrorIndicator = true;                
            }
            header.TransportErrorIndicator = ((buffer[1] & 0x80) == 0x80);
            //if (header.TransportErrorIndicator)
            //    return;
            header.PayloadUnitStartIndicator = ((buffer[1] & 0x40) == 0x40);
            header.TransportPriority = ((buffer[1] & 0x20) == 0x20);
            header.Pid = (((buffer[1] & 0x1F) << 8) + buffer[2]);
            header.TransportScramblingControl = (byte)(buffer[3] & 0xC0);
            header.AdaptionFieldControl = (byte)((buffer[3] >> 4) & 0x3);
            header.HasAdaptionField = (buffer[3] & 0x20) == 0x20;
            header.HasPayload = (buffer[3] & 0x10) == 0x10;
            header.ContinuityCounter = (byte)(buffer[3] & 0x0F);
            header.AdaptionFieldLength = 0;
            header.PayLoadStart = 4;
            if (header.HasAdaptionField)
            {
                header.AdaptionFieldLength = buffer[4];
                header.PayLoadStart = (byte)(5 + header.AdaptionFieldLength);
            }
            return header;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Transportstream Header .\n");
            sb.AppendFormat("Sync Byte: {0} .\n", SyncByte.ToString());
            sb.AppendFormat("Transport Error Indicator: {0} .\n", TransportErrorIndicator);
            sb.AppendFormat("Payload Unit Start Indicator: {0} .\n",PayloadUnitStartIndicator);
            sb.AppendFormat("Transport Priority: {0} .\n", TransportPriority);
            sb.AppendFormat("PID: {0} .\n", Pid);
            sb.AppendFormat("Transport Scrambling Control: {0} .\n", TransportScramblingControl);
            sb.AppendFormat("Adaption Field Control: {0} .\n", AdaptionFieldControl);
            sb.AppendFormat("Continuity Counter: {0} .\n" , ContinuityCounter);
            sb.AppendFormat(".\n");
            return sb.ToString();
        }
    }

}
