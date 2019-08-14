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
using System.Collections.Generic;
using System.Threading;

namespace SatIp
{
    public class PATParser : TsSectionDecoder 
    {
        #region Fields
        private IPidFilter _callback;
        private List<PMTParser> pmtParsers;
        private bool patReady;
        private bool pmtReady;
        #endregion 
        #region Properties
        public int TransportStreamId { get; private set; }
        public List<PMTParser> PmtParsers { get => pmtParsers; set => pmtParsers = value; }
        public bool PatReady { get => patReady; set => patReady = value; }
        public bool IsReady;
        #endregion 
        #region Constructor
        public PATParser(IPidFilter callback)
        {
            _callback = callback;
            IsReady = false;
            patReady = false;
            pmtReady = false;            
            pmtParsers = new List<PMTParser>();
            Pid = 0;
            TableId = 0;
        }
        #endregion
        #region Overrides
        public override void OnNewSection(TsSection section)
        {
            if (section.table_id != TableId) return;
            int pmtCount = 0;
            int loop = (section.section_length - 9) / 4;
            for (int i = 0; i < loop; i++)
            {
                int offset = (8 + (i * 4));
                int program_nr = ((section.Data[offset]) << 8) + section.Data[offset + 1];
                int pmt_pid = ((section.Data[offset + 2] & 0x1F) << 8) + section.Data[offset + 3];

                if (pmt_pid <= 0x10 || pmt_pid > 0x1FFF)
                    continue;
                if(_callback!= null)
                {
                    Thread.Sleep(250);
                    _callback.AddPid(pmt_pid);
                }
                pmtParsers.Add(new PMTParser(pmt_pid, program_nr));
                pmtCount++;
            }
            patReady = true;            
        }
        public override void OnTsPacket(byte[] tsPacket)
        { 
            if (IsReady)
                return;
            if (patReady)
            {
                pmtReady = true;
                foreach (PMTParser pmtp in pmtParsers)
                {
                    pmtp.OnTsPacket(tsPacket);
                    if (!pmtp.IsReady)
                        pmtReady = false;
                }
                if (pmtReady)                {
                    //if (_callback != null) { _callback.RemovePid(base.Pid); }
                    IsReady = true;
                }
            }
            else
                base.OnTsPacket(tsPacket);
        }
        #endregion
        #region Methods
        public List<ushort> GetPmtStreamPids()
        {
            List<ushort> streamPids = new List<ushort>();
            foreach (PMTParser pmtp in pmtParsers)
                streamPids.AddRange(pmtp.streamPids);
            return streamPids;
        }
        public void Reset()
        {
            base.Reset();
            IsReady = false;
            patReady = false;
            pmtReady = false;            
            foreach (PMTParser pmtp in pmtParsers)
                pmtp.Reset();
        }
        #endregion
    }
}
