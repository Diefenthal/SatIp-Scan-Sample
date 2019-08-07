﻿/*  
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
namespace SatIp
{
    public class TsSection
    {
        
            public static int MAX_SECTION_LENGTH = 4300;

            #region Public vars
            public int table_id;
            public int table_id_extension;
            public int section_length;
            public int section_number;
            public int version_number;
            public bool current_next_indicator;
            public int section_syntax_indicator;
            public int last_section_number;

            public int BufferPos;
            public byte[] Data;
            #endregion

            public TsSection()
            {
                Data = new byte[MAX_SECTION_LENGTH * 5];
                Reset();
            }

            public void Reset()
            {
                table_id = -1;
                table_id_extension = -1;
                section_length = -1;
                section_number = -1;
                version_number = -1;
                section_syntax_indicator = -1;
                BufferPos = 0;
                for (int i = 0; i < Data.Length; i++)
                    Data[i] = 0xFF;
            }
            public int CalcSectionLength(byte[] tsPacket, int start)
            {
                if (BufferPos < 3)
                {
                    byte bHi = 0;
                    byte bLow = 0;
                    if (BufferPos == 1)
                    {
                        bHi = tsPacket[start];
                        bLow = tsPacket[start + 1];
                    }
                    else if (BufferPos == 2)
                    {
                        bHi = Data[1];
                        bLow = tsPacket[start];
                    }
                    section_length = (int)(((bHi & 0xF) << 8) + bLow);
                }
                else
                    section_length = (int)(((Data[1] & 0xF) << 8) + Data[2]);
                return section_length;
            }
            public bool DecodeHeader()
            {
                if (BufferPos < 8)
                    return false;
                table_id = (int)Data[0];
                section_syntax_indicator = (int)((Data[1] >> 7) & 1);
                if (section_length == -1)
                    section_length = (int)(((Data[1] & 0xF) << 8) + Data[2]);
                table_id_extension = ((Data[3] << 8) + Data[4]);
                version_number = (int)((Data[5] >> 1) & 0x1F);
                current_next_indicator = (Data[5] & 0x01) != 0;
                section_number = (int)Data[6];
                last_section_number = (int)Data[7];
                return true;
            }
            public bool SectionComplete()
            {
                if (!DecodeHeader() && BufferPos > section_length && section_length > 0)
                    return true;
                if (!DecodeHeader())
                    return false;
                return (BufferPos >= section_length);
            }
        }
    }
