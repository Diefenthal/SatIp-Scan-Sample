using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public class Channel
    {
        public int Frequency;
        public string ServiceName;
        public string ServiceProvider;
        public int ServiceType;
        public int ServiceId;
        public bool Schedule;
        public bool PresentFollow;
        public RunningStatus Status;
        public bool Scrambled;
        public int TransportStreamId;
        public int OriginalNetworkId;
        public int LogicalChannelNumber;

        public short PCRPid;
        public short VideoPid;
        public short AudioPid;
        public short AC3Pid;
        public short EAC3Pid;
        public short AACPid;
        public short DTSPid;
        public short TTXPid;
        public short SubTitlePid;
    }
}
