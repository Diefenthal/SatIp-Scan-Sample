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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SatIp.Properties;

namespace SatIp.Usercontrols
{
    public partial class DeviceInformation : UserControl
    {
        private SatIpDevice device;

        public DeviceInformation()
        {
            InitializeComponent();
        }

        public DeviceInformation(SatIpDevice device)
        {
            InitializeComponent();
            this.device = device;
            tbxDeviceType.Text = device.DeviceType;
            tbxFriendlyName.Text = device.FriendlyName;
            tbxManufacture.Text = device.Manufacturer;
            tbxModeName.Text = device.ModelName;
            tbxModelDescription.Text = device.ModelDescription;
            tbxModelNumber.Text = device.ModelNumber;
            tbxModelUrl.Text = device.ModelUrl;
            tbxSerialNumber.Text = device.SerialNumber;
            tbxUniqueDeviceName.Text = device.UniqueDeviceName;
            tbxManufactureUrl.Text = device.ManufacturerUrl;
            tbxPresentationUrl.Text = device.PresentationUrl;
            pbxDVBC.Image = Resources.dvb_c;
            pbxDVBC.Visible = device.SupportsDVBC;
            pbxDVBS.Image = Resources.dvb_s;
            pbxDVBS.Visible = device.SupportsDVBS;
            pbxDVBT.Image = Resources.dvb_t;
            pbxDVBT.Visible = device.SupportsDVBT;

            try
            {
                var imageUrl =
                    string.Format(device.FriendlyName == "OctopusNet" ? "http://{0}:{1}/{2}" : "http://{0}:{1}{2}",
                        device.BaseUrl.Host, device.BaseUrl.Port, device.GetImage(1));
                pbxManufactureBrand.LoadAsync(imageUrl);
                pbxManufactureBrand.Visible = true;
            }
            catch
            {
                pbxManufactureBrand.Visible = false;
            }
        }
    }
}
