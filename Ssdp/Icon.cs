/*  
    Copyright (C) <2007-2016>  <Kay Diefenthal>

    SatIp.DiscoverySample is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SatIp.DiscoverySample is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SatIp.DiscoverySample.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace SatIp
{
    public class Icon
    {
        public Icon()
        {
            Url = "";
            MimeType = "";
        }

        public int Depth { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
    }
}
