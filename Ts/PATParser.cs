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
