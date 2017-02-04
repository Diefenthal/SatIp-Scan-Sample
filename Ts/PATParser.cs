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
            _programcount = (section.section_length - 9) / 4;
            for (var i = 9; i < _programcount; ++i)
            {

                var prgId = (section.Data[i * 4] << 8) + section.Data[(i * 4) + 1];                
                var pmtId = ((section.Data[(i * 4) + 2] & 0x1F) << 8) + section.Data[(i * 4) + 3];
                if (pmtId == 0)
                {
                    NetworkPid = prgId;
                }
                if (!_programNumbers.ContainsKey(prgId))
                {
                    _programNumbers.Add(prgId, pmtId);
                }
            }
            IsReady = true;
        }
        public void OnTsPacket(byte[] tsPacket)
        {
            _sectionDecoder.OnTsPacket(tsPacket);
        }
    }
}
