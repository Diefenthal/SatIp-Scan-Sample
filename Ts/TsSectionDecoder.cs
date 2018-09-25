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
using System;

namespace SatIp
{
    public class TsSectionDecoder
    {
        public static uint incompleteSections;
        private ushort _pid;
        private TsSection _section;
        private int _tableId;

        public event MethodOnSectionDecoded OnSectionDecoded;

        public TsSectionDecoder()
        {
            _pid = 8191;
            _tableId = -1;
            _section = new TsSection();
        }
        public TsSectionDecoder(ushort pid, int table_id)
        {
            _pid = pid;
            _tableId = table_id;
            _section = new TsSection();
        }
        
        private int AddToSection(byte[] tsPacket, int index, int sectionLen)
        {
            int num = -1;
            int length = -1;
            if ((index + sectionLen) < 0xb9)
            {
                length = sectionLen + 3;
                num = (index + sectionLen) + 3;
            }
            else
            {
                num = 0xbc;
                length = 0xbc - index;
            }
            Array.Copy(tsPacket, index, _section.Data, _section.BufferPos, length);
            _section.BufferPos += length;
            _section.DecodeHeader();
            return num;
        }

        public virtual void OnNewSection(TsSection section)
        {
        }

        public virtual void OnTsPacket(byte[] tsPacket)
        {
            TsHeader header = TsHeader.Decode(tsPacket);
            if (((_pid < 0x1fff) && (header.Pid == _pid)) && header.HasPayload)                
            {
                int payLoadStart = header.PayLoadStart;
                int num2 = 0;
                if (header.PayloadUnitStartIndicator)
                {
                    num2 = (payLoadStart + tsPacket[payLoadStart]) + 1;
                    if (_section.BufferPos == 0)
                    {
                        payLoadStart += tsPacket[payLoadStart] + 1;
                    }
                    else
                    {
                        payLoadStart++;
                    }
                }
                while (payLoadStart < 0xbc)
                {
                    if (_section.BufferPos == 0)
                    {
                        if (!header.PayloadUnitStartIndicator)
                        {
                            return;
                        }
                        if (tsPacket[payLoadStart] == 0xff)
                        {
                            return;
                        }
                        int sectionLen = this.SnapshotSectionLength(tsPacket, payLoadStart);
                        payLoadStart = this.StartNewTsSection(tsPacket, payLoadStart, sectionLen);
                    }
                    else
                    {
                        if (_section.section_length == -1)
                        {
                            _section.CalcSectionLength(tsPacket, payLoadStart);
                        }
                        if (_section.section_length == 0)
                        {
                            _section.Reset();
                            return;
                        }
                        int num4 = _section.section_length - _section.BufferPos;
                        if ((num2 != 0) && ((payLoadStart + num4) > num2))
                        {
                            num4 = num2 - payLoadStart;
                            payLoadStart = this.AddToSection(tsPacket, payLoadStart, num4);
                            _section.section_length = _section.BufferPos - 1;
                            payLoadStart = num2;
                            incompleteSections++;
                        }
                        else
                        {
                            payLoadStart = this.AddToSection(tsPacket, payLoadStart, num4);
                        }
                    }
                    if (_section.SectionComplete() && (_section.section_length > 0))
                    {
                        OnNewSection(_section);
                        OnSectionDecoded?.Invoke(_section);
                        _section.Reset();
                    }
                    num2 = 0;
                }
            }
        }

        public void Reset()
        {
            _section.Reset();
        }

        private int SnapshotSectionLength(byte[] tsPacket, int start)
        {
            if (start > 0xb8)
            {
                return -1;
            }
            return (((tsPacket[start + 1] & 15) << 8) + tsPacket[start + 2]);
        }

        private int StartNewTsSection(byte[] tsPacket, int index, int sectionLen)
        {
            int num = -1;
            int length = -1;
            if (sectionLen > -1)
            {
                if ((index + sectionLen) < 0xb9)
                {
                    length = sectionLen + 3;
                    num = (index + sectionLen) + 3;
                }
                else
                {
                    num = 0xbc;
                    length = 0xbc - index;
                }
            }
            else
            {
                num = 0xbc;
                length = 0xbc - index;
            }
            _section.Reset();
            Array.Copy(tsPacket, index, _section.Data, 0, length);
            _section.BufferPos = length;
            _section.DecodeHeader();
            return num;
        }

        public ushort Pid
        {
            get
            {
                return _pid;
            }
            set
            {
                _pid = value;
            }
        }

        public int TableId
        {
            get
            {
                return _tableId;
            }
            set
            {
                _tableId = value;
            }
        }

        public delegate void MethodOnSectionDecoded(TsSection section);
    }
}
