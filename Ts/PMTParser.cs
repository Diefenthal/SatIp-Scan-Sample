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
using System;
using System.Collections.Generic;
using System.Text;

namespace SatIp
{
    public class PMTParser : TsSectionDecoder
    {
        public bool IsReady;        
        public List<ushort> streamPids;        
        private int service_id;
        public PMTParser(int pmtPid, int service_id)
        {
            IsReady = false;            
            this.service_id = service_id;
            TableId = 0x2;
            Pid = (ushort)pmtPid;
            streamPids = new List<ushort>();
        }
        public override void OnNewSection(TsSection sections)
        {
            if (IsReady) return;

            if (sections.table_id_extension != service_id) return;
            if (sections.table_id_extension == 6033)
            {
                int xc = 564;
            }
            byte[] section = sections.Data;
            int section_length = sections.section_length;
            int pcrPid = ((section[8] & 0x1F) << 8) + section[9];            
            int program_info_length = ((section[10] & 0xF) << 8) + section[11];
            // Skip the descriptors (if any).
            int ndx = 12;
            ndx += program_info_length;
            // Now we have the actual program data.
            while (ndx < section_length - 3)
            {
                int stream_type = section[ndx++];
                int pid = ((section[ndx++] & 0x1f) << 8) + section[ndx++];
                int es_descriptors_length = ((section[ndx++] & 0x0f) << 8) + section[ndx++];                    
                    if (es_descriptors_length > 0)
                    {
                        int off = 0;
                        while (off < es_descriptors_length)
                        {
                            int descriptor_tag = section[ndx + off];
                            int descriptor_len = section[ndx + off + 1];
                            switch (descriptor_tag)
                            {
                                case 0x5:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x") + " - Registration descriptor: " + StringUtils.getString468A(section, ndx + off + 2, descriptor_len));
                                    break;
                                case 0x9: // CA Descriptor
                                    int ca_system_id = (section[ndx + off + 2] << 8) + section[ndx + off + 3];
                                    int ca_pid = ((section[ndx + off + 4] & 0x1f) << 8) + section[ndx + off + 5];
                                    //node.Nodes.Add("CA: Pid: 0x" + ca_pid.ToString("x") + " " + StringUtils.CA_System_ID2Str(ca_system_id));
                                    break;
                                case 0x0A: // ISO_639_language
                                    //node.Nodes.Add("ISO_639_language: " + StringUtils.getString468A(section, ndx + off + 2, 3));
                                    break;
                                case 0x52:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x") + " - stream identifier descriptor 0x" + section[ndx + off + 2].ToString("x"));
                                    break;
                                case 0x56: // Teletext
                                    //node.Text = "pid: 0x" + pid.ToString("x") + " [Teletext] " + StringUtils.StreamTypeToStr(stream_type);
                                    break;
                                case 0x59: // Subtitles
                                    //node.Text = "pid: 0x" + pid.ToString("x") + " [Subtitles] " + StringUtils.StreamTypeToStr(stream_type);
                                    break;
                                case 0x6A: // AC3
                                    //node.Text = "pid: 0x" + pid.ToString("x") + " [AC3-Audio] " + StringUtils.StreamTypeToStr(stream_type);
                                    break;
                                case 0x5F: // private data
                                    //node.Text = "pid: 0x" + pid.ToString("x") + " [Private Data] " + StringUtils.StreamTypeToStr(stream_type);
                                    break;
                                case 0xA1:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x") + " - ATSC: service location");
                                    break;
                                case 0xA2:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x") + " - ATSC: time shifted service");
                                    break;
                                case 0xA3:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x") + " - ATSC: component name");
                                    break;
                                default:
                                    //node.Nodes.Add("0x" + descriptor_tag.ToString("x"));
                                    break;
                            }
                            off += descriptor_len + 2;
                        }                    
                }
                ndx += es_descriptors_length;
                if (!streamPids.Contains((ushort)pid))
                    streamPids.Add((ushort)pid);
            }            
            IsReady = true;
        }
        public void Reset()
        {
            IsReady = false;            
            streamPids.Clear();
        }
    }        
 }

    
