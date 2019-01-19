/*  
    Copyright (C) <2007-2019>  <Kay Diefenthal>

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
using System.Collections.ObjectModel;
using System.Text;

namespace SatIp
{
    public class RtcpSenderReportPacket : RtcpPacket
    {
        #region Properties
        /// <summary>
        /// Get the synchronization source.
        /// </summary>
        public int SynchronizationSource { get; private set; }
        /// <summary>
        /// Get the NPT timestamp.
        /// </summary>
        public long NPTTimeStamp { get; private set; }
        /// <summary>
        /// Get the RTP timestamp.
        /// </summary>
        public int RTPTimeStamp { get; private set; }
        /// <summary>
        /// Get the packet count.
        /// </summary>
        public int SenderPacketCount { get; private set; }
        /// <summary>
        /// Get the octet count.
        /// </summary>
        public int SenderOctetCount { get; private set; }
        /// <summary>
        /// Get the list of report blocks.
        /// </summary>
        public Collection<ReportBlock> ReportBlocks { get; private set; }
        /// <summary>
        /// Get the profile extension data.
        /// </summary>
        public byte[] ProfileExtension { get; private set; } 
        #endregion

        public override void Parse(byte[] buffer, int offset)
        {
            base.Parse(buffer, offset);
            SynchronizationSource = Utils.Convert4BytesToInt(buffer, offset + 4);
            NPTTimeStamp = Utils.Convert8BytesToLong(buffer, offset + 8);
            RTPTimeStamp = Utils.Convert4BytesToInt(buffer, offset + 16);
            SenderPacketCount = Utils.Convert4BytesToInt(buffer, offset + 20);
            SenderOctetCount = Utils.Convert4BytesToInt(buffer, offset + 24);

            ReportBlocks = new Collection<ReportBlock>();
            int index = 28;

            while (ReportBlocks.Count < ReportCount)
            {
                ReportBlock reportBlock = new ReportBlock();
                reportBlock.Process(buffer, offset + index);                
                ReportBlocks.Add(reportBlock);
                index += reportBlock.BlockLength;
            }

            if (index < Length)
            {
                ProfileExtension = new byte[Length - index];

                for (int extensionIndex = 0; index < Length; index++)
                {
                    ProfileExtension[extensionIndex] = buffer[offset + index];
                    extensionIndex++;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Sender Report.\r\n");
            sb.AppendFormat("Version : {0} .\r\n", Version);
            sb.AppendFormat("Padding : {0} .\r\n", Padding);
            sb.AppendFormat("Report Count : {0} .\r\n", ReportCount);
            sb.AppendFormat("PacketType: {0} .\r\n", Type);
            sb.AppendFormat("Length : {0} .\r\n", Length);
            sb.AppendFormat("SynchronizationSource : {0} .\r\n", SynchronizationSource);
            sb.AppendFormat("NTP Timestamp : {0} .\r\n", Utils.NptTimestampToDateTime(NPTTimeStamp));
            sb.AppendFormat("RTP Timestamp : {0} .\r\n", RTPTimeStamp);
            sb.AppendFormat("Sender PacketCount : {0} .\r\n", SenderPacketCount);
            sb.AppendFormat("Sender Octet Count : {0} .\r\n", SenderOctetCount);            
            sb.AppendFormat(".\r\n");
            return sb.ToString();
        }
    }
}
