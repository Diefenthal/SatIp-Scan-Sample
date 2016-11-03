/*  
    Copyright (C) <2007-2016>  <Kay Diefenthal>

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
