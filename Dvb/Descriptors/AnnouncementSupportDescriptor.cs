using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp.Analyzer.DVB.Descriptors
{
    public class AnnouncementSupportDescriptor : Descriptor
    {
        public override void Parse(byte[] buffer, int offset)
        {
            base.Parse(buffer, offset);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
