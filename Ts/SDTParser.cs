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
    public class SDTParser
    {
        private TsSectionDecoder dec1;
        private TsSectionDecoder dec2;
        public bool IsReady;
        private Dictionary<int, ServiceDescription> _serviceDescriptions = new Dictionary<int, ServiceDescription>();
        private ServiceDescription _serviceDescription = null;
        public  int TransportStreamId;
        public SDTParser()
        {
            IsReady = false;

            this.dec1 = new TsSectionDecoder(0x11, 0x42);
            this.dec1.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
            this.dec2 = new TsSectionDecoder(0x11, 0x46);
            this.dec2.OnSectionDecoded += new TsSectionDecoder.MethodOnSectionDecoded(this.OnNewSection);
        }

        public int[] Services
        {
            get
            {
                int[] retServices = new int[ServiceCount];
                int i = 0;
                foreach (int key in _serviceDescriptions.Keys)
                {
                    retServices[i] = key;
                    i++;
                }
                return retServices;
            }
        }
        public int ServiceCount
        {
            get
            {
                return _serviceDescriptions.Keys.Count;
            }
        }
        public ServiceDescription GetServiceDescription(int service_id)
        {
            return _serviceDescriptions[service_id];
        }

        public void OnNewSection(TsSection section)
        {
            TransportStreamId = section.table_id_extension;
            int OriginalNetworkID = (section.Data[8] << 8) + section.Data[9];
            int offset = 11;
            while (offset < section.section_length - 4)
            {
                _serviceDescription = new ServiceDescription();
                _serviceDescription.ServiceID = (ushort)((section.Data[offset] << 8) + section.Data[offset + 1]);
                _serviceDescription.EitScheduleFlag = ((section.Data[offset + 2] & 0x02) != 0);
                _serviceDescription.EitPresentFollowingFlag = ((section.Data[offset + 2] & 0x01) != 0);
                _serviceDescription.RunningStatus = (RunningStatus)((section.Data[offset + 3] >> 5) & 0x07);
                _serviceDescription.FreeCaMode = (((section.Data[offset + 3] >> 4) & 0x01) != 0);
                var DescriptorsLoopLength = (ushort)(((section.Data[offset + 3] << 8) | section.Data[offset + 4]) & 0xfff);
                offset += 5;
                int descOffset = 0;
                //offset += DescriptorsLoopLength;
                while (descOffset < DescriptorsLoopLength)
                {
                    var descriptor_tag = section.Data[offset + descOffset];
                    var descriptor_length = section.Data[offset + descOffset];
                    if(DescriptorsLoopLength >0)
                    { 
                        switch (descriptor_tag)
                        {
                            case 0x42: // stuffing_descriptor
                                break;
                            case 0x48: // service_descriptor
                                ReadServiceDescriptor(section.Data, offset + descOffset + 2);
                                break;
                            case 0x49: // country_availability_descriptor
                                break;
                            case 0x4A: // linkage_descriptor
                                ReadLinkageDescriptor(section.Data, offset + descOffset + 2, descriptor_length);
                                break;
                            case 0x4B: // NVOD_reference_descriptor
                                break;
                            case 0x4C: // time_shifted_service_descriptor
                                break;
                            case 0x50: // component_descriptor
                                ReadComponentDescriptor(section.Data, offset + descOffset + 2, descriptor_length);
                                break;
                            case 0x51: // mosaic_descriptor
                                break;
                            case 0x53: // CA_identifier_descriptor
                                ReadCAIdentifierDescriptor(section.Data, offset + descOffset + 2, descriptor_length);
                                break;
                            case 0x57: // telephone_descriptor
                                break;
                            case 0x5D: // multilingual_service_name_descriptor
                                break;
                            case 0x5F: // private_data_specifier_descriptor
                                ReadPrivateDataSpecifierDescriptor(section.Data, offset + descOffset + 2, descriptor_length);
                                break;
                            case 0x64: // data_broadcast_descriptor
                                break;
                            case 0x6E: // announcement_support_descriptor
                                break;
                            case 0x71: // service_identifier_descriptor
                                break;
                            case 0x72: // service_availability_descriptor
                                break;
                            case 0x73: // default_authority_descriptor
                                break;
                            case 0x7D: // XAIT location descriptor
                                break;
                            case 0x7E: // FTA_content_management_descriptor
                                break;
                            case 0x7F: // extension descriptor
                                break;
                            default:
                                Console.WriteLine("SDT Descriptor UNKNOWN: 0x{0:X2}", descriptor_tag);
                                break;
                        }
                        descOffset += descriptor_length + 2;
                    }
                
                }
                if (!_serviceDescriptions.ContainsKey(_serviceDescription.ServiceID))
                {
                    _serviceDescriptions.Add(_serviceDescription.ServiceID, _serviceDescription);
                }
                offset += DescriptorsLoopLength;
            }
            
            IsReady = true;
        }

        public void OnTsPacket(byte[] tsPacket)
        {
            this.dec1.OnTsPacket(tsPacket);
            this.dec2.OnTsPacket(tsPacket);
        }
        public void ReadServiceDescriptor(byte[] data, int offset)
            {
                _serviceDescription.ServiceType = data[offset];  
                byte ProviderNameLength = data[offset + 1];
                _serviceDescription.ProviderName = Utils.ReadString(data, offset + 2, (int)ProviderNameLength);
                byte ServiceNameLength = data[offset + 2 + ProviderNameLength];
                _serviceDescription.ServiceName = Utils.ReadString(data, offset + 3 + ProviderNameLength, ServiceNameLength); 
            }
        private void ReadComponentDescriptor(byte[] data, int offset, byte descriptor_length)
        {
            int stream_content = data[offset] & 0x0F;
            byte component_type = data[offset + 1];
            byte component_tag = data[offset + 2];
            string language_code = Utils.ReadString(data, offset + 3, 3);
            string description = Utils.ReadString(data, offset + 6, descriptor_length - 6);
        }
        private int ReadPrivateDataSpecifierDescriptor(byte[] buffer, int offset, byte descriptorLength)
        {
            int privatedataspecifier = ((buffer[offset] << 24) + (buffer[offset + 1] << 16) + (buffer[offset + 2] << 8) + buffer[offset + 3]);
            return descriptorLength;
        }
        private void ReadLinkageDescriptor(byte[] data, int offset, byte descriptor_length)
        {
            var TransportStreamId = (ushort)((data[0] << 8) + data[1]);
            var OriginalNetworkId = (ushort)((data[2] << 8) + data[3]);
            var ServiceId = (ushort)((data[4] << 8) + data[5]);
            var LinkageType = data[6];
        }
        
        private void ReadDataBroadcastDescriptor(byte[] data, int offset, byte descriptor_length)
        { }
        
        private void ReadCAIdentifierDescriptor(byte[] data, int offset, byte descriptor_length)
        {
            var lastindex = offset + 2;
            var al = new List<ushort>();
            for (int offset2 = lastindex; offset2 < lastindex + descriptor_length - 1; offset2 += 2)
                al.Add((ushort)((data[offset2] << 8) | data[offset2 + 1]));
            var CaSystemIds = (ushort[])al.ToArray();
        }
    }

    public class ServiceDescription
    {
        public int ServiceID;
        public bool EitScheduleFlag;
        public bool EitPresentFollowingFlag;
        public RunningStatus RunningStatus;
        public bool FreeCaMode;
        public int ServiceType;
        public string ProviderName;
        public string ServiceName;
    }
    public enum RunningStatus
    {
        UNDEFINED = 0,
        NOT_RUNNING = 1,
        STARTS_IN_FEW_SECONDS = 2,
        PAUSING = 3,
        RUNNING = 4,
        RESERVED_1 = 5,
        RESERVED_2 = 6,
        RESERVED_3 = 7,
    }
}
