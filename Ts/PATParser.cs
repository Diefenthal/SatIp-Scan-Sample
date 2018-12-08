/*  
    Copyright (C) <2007-2017>  <Kay Diefenthal>

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
using System.Collections.Generic;

namespace SatIp
{
    public class PATParser
    {
        private TsSectionDecoder _sectionDecoder;
        private List<short> _pids = new List<short>();
        private Dictionary<int, int> _programNumbers = new Dictionary<int, int>();
        public int NetworkPid;

        public bool HasNitReference { get; private set; }
        public int TransportStreamId { get; private set; }

        private int _programcount;
        public bool IsReady;

        public PATParser()
        {
            _sectionDecoder = new TsSectionDecoder(0x00, 0x00);
            _sectionDecoder.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
            IsReady = false;

        }
        public Dictionary<int, int> Programs
        {
            get
            {
                return _programNumbers;
            }
        }


        public void OnNewSection(TsSection section)
        {
            TransportStreamId = section.table_id_extension;
            int pmtCount = 0;
            int loop = (section.section_length - 9) / 4;
            for (int i = 0; i < loop; i++)
            {
                int offset = (8 + (i * 4));
                int program_nr = ((section.Data[offset]) << 8) + section.Data[offset + 1];
                int pmt_pid = ((section.Data[offset + 2] & 0x1F) << 8) + section.Data[offset + 3];

                if (program_nr == 0)
                {
                    NetworkPid = pmt_pid;
                    HasNitReference = true;
                }
                if (!_programNumbers.ContainsKey(program_nr))
                {
                    _programNumbers.Add(program_nr, pmt_pid);
                }
            }
            pmtCount++;

            IsReady = true;
        }
        public void OnTsPacket(byte[] tsPacket)
        {
            _sectionDecoder.OnTsPacket(tsPacket);
        }
        public void Reset()
        {
            
            IsReady = false;
            
        }
    }
}
