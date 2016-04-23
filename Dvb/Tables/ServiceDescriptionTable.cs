using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SatIp.Analyzer.DVB.Descriptors;

namespace SatIp.Analyzer
{
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
    public class ServiceDescription
    {
        public ushort ServiceID;
        public byte Reserved;
        public bool EitScheduleFlag;
        public bool EitPresentFollowingFlag;
        public RunningStatus RunningStatus;
        public bool FreeCaMode;
        public ushort DescriptorsLoopLength;
        public Descriptor[] Descriptors;
    }
    public class ServiceDescriptionTable
    {        
        public int TableId;
        public int SyntaxIndicator;
        public int Length;
        public int TransportStreamId;
        public int VersionNumber;
        public int CurrentNextIndicator;
        public int SectionNumber;
        public int LastSectionNumber;
        public int OriginalNetworkId;
        List<ServiceDescription> Services;

        public ServiceDescriptionTable()
        {
            
        }
        public static ServiceDescriptionTable Decode(bool start, int point, byte[] buffer)
        {
            var sdt= new  ServiceDescriptionTable();
            sdt.TableId = buffer[point + 1];
            sdt.SyntaxIndicator = (buffer[point + 1] >> 7) & 1;
            sdt.Length = ((buffer[point + 2] & 15) * 0x100) + buffer[point + 3];
            sdt.TransportStreamId = (buffer[point + 4] * 0x100) + buffer[point + 5];
            sdt.VersionNumber = buffer[point + 8];
            sdt.CurrentNextIndicator = buffer[point + 5] & 1;
            sdt.LastSectionNumber = buffer[point + 8];
            sdt.SectionNumber = buffer[point + 7];
            sdt.OriginalNetworkId = (buffer[point + 8] << 8) + buffer[point + 9];
            sdt.Services = new List<ServiceDescription>();
            int offset = 12;
            while (offset < sdt.Length - 1)
            {
                ServiceDescription servicedescription = new ServiceDescription();                
                servicedescription.ServiceID = (ushort)((buffer[offset] << 8) + buffer[offset + 1]);
                servicedescription.Reserved = (byte)(buffer[offset + 2] >> 2);
                servicedescription.EitScheduleFlag = ((buffer[offset + 2] & 0x02) != 0);
                servicedescription.EitPresentFollowingFlag = ((buffer[offset + 2] & 0x01) != 0);
                servicedescription.RunningStatus = (RunningStatus)((buffer[offset + 3] >> 5) & 0x07);
                servicedescription.FreeCaMode = (((buffer[offset + 3] >> 4) & 0x01) != 0);
                servicedescription.DescriptorsLoopLength = (ushort)(((buffer[offset + 3] & 0x0F) << 8) | buffer[offset + 4]);
                //servicedescription.Descriptors = Descriptor.ParseDescriptors(buffer, offset + 5, servicedescription.DescriptorsLoopLength);                
                sdt.Services.Add(servicedescription);
                offset += servicedescription.DescriptorsLoopLength + 5;
            }
            return sdt;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Service Description Table \n");
            sb.AppendFormat("Table ID : {0} \n", TableId);
            sb.AppendFormat("Syntax Indicator : {0} \n", SyntaxIndicator);
            sb.AppendFormat("Length : {0} \n", Length);
            sb.AppendFormat("Transport Stream Id: {0} \n", TransportStreamId);
            sb.AppendFormat("Version Number : {0} \n", VersionNumber);
            sb.AppendFormat("Current / Next Indicator : {0} \n", CurrentNextIndicator);
            sb.AppendFormat("Section Number : {0} \n", SectionNumber);
            sb.AppendFormat("Last Section Number : {0} \n", LastSectionNumber);
            sb.AppendFormat("OriginalNetwork Id : {0} \n", OriginalNetworkId);
            foreach (var service in Services)
            {
                sb.AppendFormat("ServiceId : {0} \n", service.ServiceID);
                sb.AppendFormat("Reserved : {0} \n", service.Reserved);
                sb.AppendFormat("EitScheduleFlag : {0} \n", service.EitScheduleFlag);
                sb.AppendFormat("EitPresentFollowingFlag : {0} \n", service.EitPresentFollowingFlag);
                sb.AppendFormat("RunningStatus : {0} \n", service.RunningStatus);
                sb.AppendFormat("FreeCaMode : {0} \n", service.FreeCaMode);
                sb.AppendFormat("DescriptorsLoopLength : {0} \n", service.DescriptorsLoopLength);
                //foreach (var descriptor in service.Descriptors)
                //{
                //    sb.AppendFormat("Descriptors : {0} \n", descriptor.ToString());
                //}
            }
            sb.AppendFormat(".\n");
            return sb.ToString();
        }
    }
}
