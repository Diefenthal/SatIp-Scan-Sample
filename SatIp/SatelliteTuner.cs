﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SatIp
{
    public class SatelliteTuner : Tuner
    {

        public override TunerType Type
        {
            get { return TunerType.Satellite; }
        }
        public override int Index
        {
            get { throw new NotImplementedException(); }
        }

        public override int SignalLevel
        {
            get { throw new NotImplementedException(); }
        }

        public override int SignalQuality
        {
            get { throw new NotImplementedException(); }
        }

        public override bool SignalLocked
        {
            get { throw new NotImplementedException(); }
        }

        public override void Tune(string parameters)
        {
            throw new NotImplementedException();
        }
    }
}
