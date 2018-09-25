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
    public class NITParser
    {
        private TsSectionDecoder dec1;
        private TsSectionDecoder dec2;
        private string networkName = "Unknown";
        private Dictionary<int, NetworkInformation> _networkInformations = new Dictionary<int, NetworkInformation>();
        private NetworkInformation _currentNetwork = null;
        public int TransportStreamId;
        public bool IsReady;
        public NITParser()
        {
            IsReady = false;

            this.dec1 = new TsSectionDecoder(0x10, 0x40);
            this.dec1.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
            //this.dec2 = new TsSectionDecoder(0x10, 0x41);
            //this.dec2.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
        }
        public int NetworkCount
        {
            get { return _networkInformations.Count; }
        }

        public NetworkInformation GetNetworkInformation(int transportStreamId)
        {
            return _networkInformations[transportStreamId];
        }

        public void OnNewSection(TsSection section)
        {
            TransportStreamId = section.table_id_extension;
            var offset = 0;
            int network_descriptors_length = ((section.Data[8] & 0x0F) << 8) + section.Data[9];

            int descOffset = offset + 2;

            while (descOffset < 10 + network_descriptors_length)
            {
                byte descriptor_tag = section.Data[descOffset];
                byte descriptor_length = section.Data[descOffset + 1];

                switch (descriptor_tag)
                {
                    case 0x40:
                        networkName = Utils.ReadString(section.Data, descOffset + 2, descriptor_length);
                        Console.WriteLine("Network Name: " + networkName);
                        break;
                    case 0x4A: // linkage descriptor
                        ReadLinkageDescriptor(section.Data, descOffset + 2, descriptor_length);
                        break;
                    case 0x5F: // private_data_specifier_descriptor   
                        ReadPrivateDataSpecifierDescriptor(section.Data, descOffset + 2, descriptor_length);
                        break;
                    default:
                        Console.WriteLine("NIT Descriptor UNKNOWN: 0x{0:X2}", descriptor_tag);
                        break;
                }

                descOffset += descriptor_length + 2;
            }

            offset = 10 + network_descriptors_length;

            int transport_stream_loop_length = ((section.Data[offset] & 0x0F) << 8) +
                                                 section.Data[offset + 1];

            offset += 2;
            descOffset = offset;
            offset += transport_stream_loop_length;
            while (descOffset < transport_stream_loop_length)
            {
                int transport_stream_id = (section.Data[descOffset] << 8) + section.Data[descOffset + 1];
                int original_network_id = (section.Data[descOffset + 2] << 8) + section.Data[descOffset + 3];
                int transport_descriptors_length = ((section.Data[descOffset + 4] & 0x0F) << 8) +
                                                     section.Data[descOffset + 5];

                _currentNetwork = new NetworkInformation
                    (
                        transport_stream_id,
                        original_network_id,
                        networkName
                    );
                if (!_networkInformations.ContainsKey(transport_stream_id))
                {
                    _networkInformations.Add(transport_stream_id, _currentNetwork);
                }

                descOffset += 6;
                int transOffset = descOffset;
                descOffset += transport_descriptors_length;
                while (transOffset < descOffset)
                {
                    byte descriptor_tag = section.Data[transOffset];
                    byte descriptor_length = section.Data[transOffset + 1];

                    switch (descriptor_tag)
                    {
                        case 0x40: // network name descriptor
                            networkName = Utils.ReadString(section.Data, descOffset + 2, descriptor_length);
                            break;
                        case 0x41: // service list descriptor
                            break;
                        case 0x42: // stuffing descriptor
                            break;
                        case 0x43: // satellite delivery system descriptor
                            ReadSatelliteDeliverySystem(section.Data, transOffset +2, descriptor_length);
                            break;
                        case 0x44: // cable delivery system descriptor
                            break;                        
                        case 0x5A: //terrestrial_delivery_system_descriptor
                            //ReadTerrestrialDeliverySystem(section.Data, transOffset + 2, descriptor_length);
                            break;
                        case 0x5B: // multilingual_network_name_descriptor                            
                            break;                        
                        case 0x62: // frequency_list_descriptor                            
                            break;
                        case 0x6C: // cell_list_descriptor                            
                            break;
                        case 0x6D: // cell_frequency_link_descriptor                            
                            break;
                        case 0x73: // default_authority_descriptor                            
                            break;
                        case 0x77: // time_slice_fec_identifier_descriptor                            
                            break;
                        case 0x79: // S2_satellite_delivery_system_descriptor                            
                            break;
                        case 0x7D: // XAIT location descriptor                            
                            break;
                        case 0x7E: // FTA_content_management_descriptor                            
                            break;
                        case 0x7F: // extension descriptor                            
                            break;
                        case 0x83:
                            ReadLCN(section.Data, transOffset + 2, descriptor_length);
                            break;                       
                        default:                            
                            break;
                    }

                    transOffset += 2 + descriptor_length;
                }

            }
            IsReady = true;
        }
        private int ReadPrivateDataSpecifierDescriptor(byte[] buffer, int offset, byte descriptorLength)
        {
            int privatedataspecifier = ((buffer[offset] << 24) + (buffer[offset + 1] << 16) + (buffer[offset + 2] << 8) + buffer[offset + 3]);
            return descriptorLength;
        }
        private int ReadLinkageDescriptor(byte[] data, int offset, byte descriptorlength)
        {
            var TransportStreamId = (ushort)((data[0] << 8) + data[1]);
            var OriginalNetworkId = (ushort)((data[2] << 8) + data[3]);
            var ServiceId = (ushort)((data[4] << 8) + data[5]);
            var LinkageType = data[6];
            return descriptorlength;
        }
        private int ReadSatelliteDeliverySystem(byte[] buffer, int offset, byte descriptorLength)
        {
            int lastIndex = offset;

            _currentNetwork.Frequency = Utils.ConvertBCDToInt(buffer, lastIndex, 8);
            
            lastIndex += 4;

            _currentNetwork.OrbitalPosition = Utils.ConvertBCDToInt(buffer, lastIndex, 4);
            lastIndex += 2;

            _currentNetwork.EastFlag = ((buffer[lastIndex] & 0x80) != 0);
            _currentNetwork.Polarization = (buffer[lastIndex] >> 5) & 0x03;
            _currentNetwork.RollOff = (buffer[lastIndex] >> 3) & 0x03;
            _currentNetwork.ModulationSystem = ((buffer[lastIndex] & 0x04) >> 2);
            _currentNetwork.ModulationType = buffer[lastIndex] & 0x03;

            lastIndex++;

            _currentNetwork.SymbolRate = Utils.ConvertBCDToInt(buffer, lastIndex, 7);
            _currentNetwork.innerFec = buffer[lastIndex + 3] & 0x17;
            //lastIndex += 4;

            return descriptorLength;
        }
        private int ReadTerrestrialDeliverySystem(byte[] buffer, int offset, byte descriptorLength)
        {
            //_currentNetwork.Frequency = ((buffer[offset] << 24) +
            //                           (buffer[offset + 1] << 16) +
            //                           (buffer[offset + 2] << 8) +
            //                            buffer[offset + 3]) / 100;

            //int bandwidth = (buffer[offset + 4] & 0xE0) >> 5;

            //// Map the bandwidth code to a number (in MHz)
            //switch (bandwidth)
            //{
            //    case 0:
            //        _currentNetwork.Bandwidth = 8;
            //        break;
            //    case 1:
            //        _currentNetwork.Bandwidth = 7;
            //        break;
            //    case 2:
            //        _currentNetwork.Bandwidth = 6;
            //        break;
            //    case 3:
            //        _currentNetwork.Bandwidth = 5;
            //        break;
            //    default:

            //        break;
            //}
            //int priority = (buffer[offset + 4] & 0x10) >> 4;
            //int time_slicing_indicator = (buffer[offset + 4] & 0x08) >> 3;
            //int MPE_FEC_indicator = (buffer[offset + 4] & 0x04) >> 2;
            //int constellation = (buffer[offset + 5] & 0xC0) >> 6;

            //// Map the constealltion code to a ModulationType
            //switch (constellation)
            //{
            //    case 0:
            //        _currentNetwork.ModulationType = "Qpsk";
            //        break;
            //    case 1:
            //        _currentNetwork.ModulationType = "16qam";
            //        break;
            //    case 2:
            //        _currentNetwork.ModulationType = "64qam";
            //        break;
            //    default:

            //        break;
            //}
            return descriptorLength;
        }
        private int ReadLCN(byte[] buffer, int offset, int descriptorLength)
        {
            int descOffset = offset;
            while (descOffset < offset + descriptorLength)
            {
                int service_id = (buffer[descOffset] << 8) + buffer[descOffset + 1];
                int lcn = ((buffer[descOffset + 2] & 0x03) << 8) + buffer[descOffset + 3];
                _currentNetwork.AddLCN(service_id, lcn);
                descOffset += 4;
            }
            return descOffset - offset;
        }
        
    
        public void OnTsPacket(byte[] tsPacket)
        {
            this.dec1.OnTsPacket(tsPacket);
            //this.dec2.OnTsPacket(tsPacket);
        }
    }
    public class NetworkInformation
    {
        private int _transportStreamId;
        private int _originalNetworkId;
        private string _networkName;
        private Dictionary<int, int> _lcns = new Dictionary<int, int>();
        internal int innerFec;

        public NetworkInformation(int transportStreamId, int originalNetworkId, string networkName)
        {            
            _transportStreamId = transportStreamId;
            _originalNetworkId = originalNetworkId;
            _networkName = networkName;
        }

        internal void AddLCN(int service_id, int lcn)
        {
            _lcns.Add(service_id, lcn);
        }
        public int LcnCount
        {
            get { return _lcns.Keys.Count; }
        }
        
        

        public int Bandwidth { get; set; }

        public int Frequency { get; set; }
        public object SymbolRate { get; internal set; }
        public int ModulationType { get; internal set; }
        public object ModulationSystem { get; internal set; }
        public int RollOff { get; internal set; }
        public int Polarization { get; internal set; }
        public bool EastFlag { get; internal set; }
        public int OrbitalPosition { get; internal set; }
    }
}
