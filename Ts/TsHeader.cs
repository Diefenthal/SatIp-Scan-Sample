/*  
    Copyright (C) <2007-2016>  <Kay Diefenthal>

    SatIp is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SatIp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SatIp.  If not, see <http://www.gnu.org/licenses/>.
*/
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
            if (!header.TransportErrorIndicator)
            {
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
            return (null);
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
