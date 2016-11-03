using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public enum TunerType
    {
        Cable,
        Satellite,
        Terrestrial
    }
    public abstract class Tuner
    {
        public abstract TunerType Type { get; }
        public abstract int Index { get; }
        public abstract int SignalLevel { get; }
        public abstract int SignalQuality { get; }
        public abstract bool SignalLocked { get; }
        public abstract void Tune(string parameters);
        
    }
}
