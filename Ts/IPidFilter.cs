using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public interface IPidFilter
    {
        void AddPid(int pid);
        void RemovePid(int pid);
    }
}
