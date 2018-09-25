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
using System.Collections.Generic;
using System.Text;

namespace SatIp
{
    public class PMTParser
    {
        public ushort PcrPID;
        public int ProgramNumber;       
        
        public bool IsReady;
        private TsSectionDecoder _sectionDecoder;
        public PMTParser(short pid)
        {
            IsReady = false;
            _sectionDecoder = new TsSectionDecoder((ushort)pid,0x02);
            _sectionDecoder.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
        }
        public void OnNewSection(TsSection section)
        {
            ProgramNumber = section.table_id_extension;
            PcrPID = (ushort)((section.Data[8] & 0x1F) << 8 | section.Data[9]);
            int program_info_length = ((section.Data[10] & 0x0F) << 8) + section.Data[11];
            var offset = 12 + program_info_length;
            while (offset < (section.section_length - 4))
            {
                byte stream_type = section.Data[offset];
                int elementary_PID = ((section.Data[offset + 1] & 0x1F) << 8) + section.Data[offset + 2];
                var pm = new ProgramMap(elementary_PID, stream_type);
                int ES_info_length = ((section.Data[offset + 3] & 0x0F) << 8) + section.Data[offset + 4];

                offset += 5;
                int descOffset = offset;
                offset += ES_info_length;
                if (offset > section.section_length)
                {
                    // Oops something must have gone wrong for this to happen
                    //Log.Write(LogLevel.ERROR, "PMTTable: Offset larger than section size");
                    //_pids.Add(pid);
                    return;
                }
                while (descOffset < offset)
                {
                    var descriptor_tag = section.Data[descOffset];
                    var descriptor_length = section.Data[descOffset];
                    switch (descriptor_tag)
                    {
                        case 0x0A: // ISO 639 language descriptor
                            int langOffset = descOffset + 2;
                            while (langOffset < descOffset + 2 + descriptor_length)
                            {
                                pm.Language = Utils.ReadString(section.Data, langOffset, 3);
                                pm.AudioType = section.Data[3 + langOffset];
                                langOffset += 4;
                            }
                            break;
                        case 0x09: // CA Descriptor
                            var SystemId = section.Data[descOffset + 2]+section.Data[descOffset + 3];
                            var CaPid = section.Data[descOffset + 4] + section.Data[descOffset + 5];
                            break;
                        case 0x45: // VBI_data_descriptor
                            break;
                        case 0x46: // VBI_teletext_descriptor
                            break;
                        case 0x51: // mosaic_descriptor
                            break;
                        case 0x52: // stream_identifier_descriptor
                            byte component_tag = section.Data[descOffset + 2];
                            break;
                        case 0x56: // teletext_descriptor
                            break;
                        case 0x59: // subtitling_descriptor
                            int lanOffset = descOffset + 2;
                            while (lanOffset < descOffset + 2 + descriptor_length)
                            {
                                var Language = Utils.ReadString(section.Data, lanOffset, 3);
                                var SubtitlingType = Utils.ReadString(section.Data,3 + lanOffset, 1);
                                var composition_page_id = Utils.ReadString(section.Data, 4 + lanOffset, 2);
                                var ancillary_page_id = Utils.ReadString(section.Data, 6 + lanOffset, 2);
                                lanOffset += 8;
                            }
                            break;
                        case 0x5F: // private_data_specifier_descriptor
                            ReadPrivateDataSpecifierDescriptor(section.Data, descOffset + 2, descriptor_length);
                            break;
                        case 0x60: // service_move_descriptor
                            break;
                        case 0x65: // scrambling_descriptor
                            break;
                        case 0x66: // data_broadcast_id_descriptor
                            break;
                        case 0x6A: // AC-3_descriptor
                            break;
                        case 0x6B: // ancillary_data_descriptor
                            break;
                        case 0x6F: // application_signalling_descriptor
                            break;
                        case 0x70: // adaptation_field_data_descriptor
                            break;
                        case 0x74: // related_content_descriptor
                            break;
                        case 0x78: // ECM_repetition_rate_descriptor
                            break;
                        case 0x7A: // enhanced_AC-3_descriptor
                            break;
                        case 0x7B: // DTS® descriptor
                            break;
                        case 0x7C: // AAC descriptor
                            break;
                        case 0x7D: // XAIT location descriptor
                            break;
                        case 0x7F: // extension descriptor
                            break;
                        default:
                            Console.WriteLine("PMT Descriptor UNKNOWN: 0x{0:X2}", descriptor_tag);
                            break;
                    }
                    descOffset += descriptor_length + 2;
                }
                //_pids.Add(pm);
            }

            IsReady = true;
        }
        public void OnTsPacket(byte[] tsPacket)
        {
            _sectionDecoder.OnTsPacket(tsPacket);
        }
        private int ReadPrivateDataSpecifierDescriptor(byte[] buffer, int offset, byte descriptorLength)
        {
            int privatedataspecifier = ((buffer[offset] << 24) + (buffer[offset + 1] << 16) + (buffer[offset + 2] << 8) + buffer[offset + 3]);
            return descriptorLength;
        }
        
    }
    public class ProgramMap
    {
        //private int _programClockReference;
        private int _programId;
        private int _streamType;
        private byte component_tag = 0;
        private string _language;
        private int _audioType;
        public ProgramMap(int programId,int streamType)
        {
            _programId = programId;
            _streamType = streamType;
        }

        public byte AudioType { get; set; }

        public string Language { get; set; }
    }
}
