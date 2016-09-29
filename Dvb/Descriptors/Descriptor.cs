using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SatIp.Analyzer.DVB.Descriptors
{
    public class Descriptor
    {
        public byte DescriptorTag;
        public byte DescriptorLength;
        private byte[] unparseData;

        public virtual void Parse(byte[] buffer, int offset)
        {
            DescriptorTag = buffer[offset];
            DescriptorLength = buffer[offset + 1];
        }

        public static Descriptor ParseDescriptor(byte[] buffer, int offset, byte length)
        {
            Descriptor descriptor;
            var tag = buffer[offset];
            switch (tag)
            {
                //case 0x42: descriptor = new StuffingDescriptor(); break;
                //case 0x45: descriptor = new VBIDataDescriptor(); break;                
                //case 0x46: descriptor = new VBITeletextDescriptor(); break;
                case 0x48: descriptor = new ServiceDescriptor(); break;
                //case 0x49: descriptor = new CountryAvailabilityDescriptor(); break;
                //case 0x4A: descriptor = new LinkageDescriptor(); break;
                //case 0x4B: descriptor = new NVODReferenceDescriptor(); break;
                //case 0x4C: descriptor = new TimeshiftedServiceDescriptor(); break;
                //case 0x50: descriptor = new ComponentDescriptor(); break;
                //case 0x51: descriptor = new MosaicDescriptor(); break;                
                case 0x52: descriptor = new StreamIndentifierDescriptor(); break;
                //case 0x53: descriptor = new CAIdentifierDescriptor(); break;
                case 0x56: descriptor = new TeletextDescriptor(); break;
                //case 0x57: descriptor = new TelephoneDescriptor(); break;
                //case 0x59: descriptor = new SubtitlingDescriptor(); break;
                //case 0x5D: descriptor = new MultilingualServiceNameDescriptor(); break;
                //case 0x5F: descriptor = new PrivateDataSpecifierDescriptor(); break;                
                //case 0x60: descriptor = new ServiceMoveDescriptor(); break;
                //case 0x64: descriptor = new DataBroadcastDescriptor(); break;
                //case 0x65: descriptor = new ScramblingDescriptor(); break;                
                case 0x66: descriptor = new DataBroadcastIdDescriptor(); break;                
                //case 0x6A: descriptor = new AC3Descriptor(); break;                
                //case 0x6B: descriptor = new AncillaryDataDescriptor(); break;
                //case 0x6E: descriptor = new AnnouncementSupportDescriptor(); break;
                case 0x6F: descriptor = new ApplicationSignallingDescriptor(); break;                
                //case 0x70: descriptor = new AdaptationFieldDataDescriptor(); break;
                //case 0x71: descriptor = new ServiceIdentifierDescriptor(); break;
                //case 0x72: descriptor = new ServiceAvailabilityDescriptor(); break;
                //case 0x73: descriptor = new DefaultAuthorityDescriptor(); break;
                //case 0x74: descriptor = new RelatedContentDescriptor(); break;
                //case 0x78: descriptor = new ECMRepetitionRateDescriptor(); break;
                //case 0x7A: descriptor = new EnhancedAC3Descriptor(); break;
                //case 0x7B: descriptor = new DTSDescriptor(); break;
                //case 0x7C: descriptor = new AACDescriptor(); break;                
                //case 0x7D: descriptor = new XAITLocationDescriptor(); break;
                //case 0x7E: descriptor = new FTAContentManagementDescriptor(); break;
                //case 0x7F: descriptor = new ExtensionDescriptor(); break;
                default: 
                     descriptor = new Descriptor(); 
                    break; 

                //    descriptor.unparseData = new byte[length];
                //    Array.Copy(buffer, offset +2 , descriptor.unparseData, 0, length);
                //    break;
            }
            descriptor.Parse(buffer,offset);
            return descriptor;
        }
        public static Descriptor[] ParseDescriptors(byte[] data, int offset, int descriptorsLength)
        {
            ArrayList al = new ArrayList();
            if (descriptorsLength >= 2)
            {
                Descriptor descriptor;
                do
                {
                    al.Add(descriptor = ParseDescriptor(data, offset, data[offset + 1]));
                    descriptorsLength -= descriptor.DescriptorLength + 2;
                    offset += descriptor.DescriptorLength + 2;
                }
                while (descriptorsLength >= 2);
            }
            return (Descriptor[])al.ToArray(typeof(Descriptor));
        }

        public override string ToString()
        {
            return ToString("");
        }
        public virtual string ToString(string prefix)
        { return ""; }
        
    }
}
