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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SatIp.Properties;

namespace SatIp
{
    public partial class DevicesForm : Form
    {
        private SSDPClient ssdp = null;   
        
        public DevicesForm()
        {
            InitializeComponent();
            Logger.SetLogFilePath("Sample.log", Settings.Default.LogLevel);
            ssdp = new SSDPClient();
            ssdp.DeviceFound += new SSDPClient.DeviceFoundHandler(DeviceFound);
            ssdp.DeviceLost += new SSDPClient.DeviceLostHandler(DeviceLost);
            ssdp.FindByType("urn:ses-com:device:SatIPServer:1");
        }

        private void DeviceFound(object sender, SatIpDeviceFoundArgs args)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    DeviceFound(sender, args);
                });
                return;
            }
            var servernode = treeView1.Nodes[0].Nodes.Add(args.Device.UniqueDeviceName, args.Device.FriendlyName);
            servernode.ToolTipText = args.Device.DeviceDescription;
            foreach (var tuner in args.Device.Tuners)
            {
                switch (tuner.Type)
                {
                    case TunerType.Cable:
                        servernode.Nodes.Add("Cable");
                        break;
                    case TunerType.Satellite:
                        servernode.Nodes.Add("Satellite");
                        break;
                    case TunerType.Terrestrial:
                        servernode.Nodes.Add("Terrestrial");
                        break;
                }                
            }
            if (treeView1.Nodes[0].IsExpanded != true)
                treeView1.Nodes[0].Expand();

        }
        private void DeviceLost(object sender, SatIpDeviceLostArgs args)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    DeviceLost(sender, args);
                });
                return;
            }
            Logger.Info("Device with UUID :{0} restarts,and will removed from the Devices Tree", args.Uuid);
            if (treeView1.Nodes[0].Nodes.ContainsKey(args.Uuid))
            {
                var tn = treeView1.Nodes[0].Nodes[args.Uuid];
                treeView1.Nodes[0].Nodes.Remove(tn);
                treeView1.Update();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var device = (SatIpDevice)ssdp.FindByUDN(e.Node.Name);
            if (device != null)
            {
                Form2 frm = new Form2(device);
                frm.ShowDialog();
            }
        }

        private void DevicesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ssdp.Dispose();
            ssdp.DeviceFound -= new SSDPClient.DeviceFoundHandler(DeviceFound);
            ssdp.DeviceLost -= new SSDPClient.DeviceLostHandler(DeviceLost);            
            ssdp = null;
        }
    }
}
