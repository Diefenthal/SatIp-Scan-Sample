using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp.Analyzer.DVB.Descriptors
{
    public class ServiceDescriptor :Descriptor
    {
        public int ServiceType;
        public string ServiceName;
        public string ProviderName;
        public int ServiceNameLength;
        public int ProviderNameLength;
        public override void Parse(byte[] buffer, int offset)
        {
            base.Parse(buffer, offset);
            ServiceType = buffer[offset + 2];
            ProviderNameLength = buffer[offset + 3];
            var providerName = new char[ProviderNameLength];
            Array.Copy(buffer, offset + 4, providerName, 0, ProviderNameLength);
            ProviderName = new string(providerName); 
            ServiceNameLength = buffer[offset + 4 + ProviderNameLength];
            var serviceName = new char[ServiceNameLength];
            Array.Copy(buffer, offset + 5+ProviderNameLength, serviceName, 0, ServiceNameLength);
            foreach (var c in serviceName) 
                 if (c >= 32) 
                     ServiceName += c; 

             
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Service Descriptor {0} \n",base.DescriptorTag);
            sb.AppendFormat("Service Descriptor Lenght {0} \n",base.DescriptorLength);
            sb.AppendFormat("Service Type {0}\n",ServiceType);
            sb.AppendFormat("Service Provider Name {0}\n",ProviderName);
            sb.AppendFormat("Service Name {0} \n",ServiceName);
            return sb.ToString();
        }
    }
}
