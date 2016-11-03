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
                int descOffset = offset;
                offset += DescriptorsLoopLength;
                while (descOffset < offset)
                {
                    var descriptor_tag = section.Data[descOffset];
                    var descriptor_length = section.Data[descOffset];
                    switch (descriptor_tag)
                    {
                        case 0x42: // stuffing_descriptor
                            break;
                        case 0x48: // service_descriptor
                            ReadServiceDescriptor(section.Data, descOffset + 2);
                            break;
                        case 0x49: // country_availability_descriptor
                            break;
                        case 0x4A: // linkage_descriptor
                            break;
                        case 0x4B: // NVOD_reference_descriptor
                            break;
                        case 0x4C: // time_shifted_service_descriptor
                            break;
                        case 0x50: // component_descriptor
                            ReadComponentDescriptor(section.Data, descOffset + 2, descriptor_length);
                            break;
                        case 0x51: // mosaic_descriptor
                            break;
                        case 0x53: // CA_identifier_descriptor
                            break;
                        case 0x57: // telephone_descriptor
                            break;
                        case 0x5D: // multilingual_service_name_descriptor
                            break;
                        case 0x5F: // private_data_specifier_descriptor
                            ReadPrivateDataSpecifierDescriptor(section.Data, descOffset + 2, descriptor_length);
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
                            break;
                    }
                    descOffset += descriptor_length + 2;
                }
                if (!_serviceDescriptions.ContainsKey(_serviceDescription.ServiceID))
                {
                    _serviceDescriptions.Add(_serviceDescription.ServiceID, _serviceDescription);
                }
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
                _serviceDescription.ProviderName = ReadString(data, offset + 2, (int)ProviderNameLength);
                byte ServiceNameLength = data[offset + 2 + ProviderNameLength];
                _serviceDescription.ServiceName = ReadString(data, offset + 3 + ProviderNameLength, ServiceNameLength); 
            }
        private void ReadComponentDescriptor(byte[] data, int offset, byte descriptor_length)
        {
            int stream_content = data[offset] & 0x0F;
            byte component_type = data[offset + 1];
            byte component_tag = data[offset + 2];
            string language_code = ReadString(data, offset + 3, 3);
            string description = ReadString(data, offset + 6, descriptor_length - 6);
        }
        private int ReadPrivateDataSpecifierDescriptor(byte[] buffer, int offset, byte descriptorLength)
        {
            int privatedataspecifier = ((buffer[offset] << 24) + (buffer[offset + 1] << 16) + (buffer[offset + 2] << 8) + buffer[offset + 3]);
            return descriptorLength;
        }
        protected string ReadString(byte[] data, int offset, int length)
        {
            string encoding = "utf-8"; // Standard latin alphabet
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                byte character = data[offset + i];
                bool notACharacter = false;
                if (i == 0)
                {
                    if (character < 0x20)
                    {
                        switch (character)
                        {
                            case 0x00:
                                break;
                            case 0x01:
                                encoding = "iso-8859-5";
                                break;
                            case 0x02:
                                encoding = "iso-8859-6";
                                break;
                            case 0x03:
                                encoding = "iso-8859-7";
                                break;
                            case 0x04:
                                encoding = "iso-8859-8";
                                break;
                            case 0x05:
                                encoding = "iso-8859-9";
                                break;
                            default:
                                break;
                        }
                        notACharacter = true;
                    }
                }
                if (character < 0x20 || (character >= 0x80 && character <= 0x9F))
                {                    
                    notACharacter = true;
                }
                if (!notACharacter)
                {
                    bytes.Add(character);
                }
            }
            Encoding enc = Encoding.GetEncoding(encoding);
            ASCIIEncoding destEnc = new ASCIIEncoding();
            byte[] destBytes = Encoding.Convert(enc, destEnc, bytes.ToArray());
            return destEnc.GetString(destBytes);
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
