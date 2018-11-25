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
