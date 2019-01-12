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
