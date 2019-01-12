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
using System.Windows.Forms;
using SatIp.Properties;
using SatIp.Usercontrols;

namespace SatIp
{
    public partial class MainForm : Form
    {        
        private SSDPClient ssdp = null;   
        
        public MainForm()
        {
            InitializeComponent();
            Logger.SetLogFilePath("SatIp Scan Sample.log", Settings.Default.LogLevel);
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
            servernode.Tag = args.Device;
            foreach (var tuner in args.Device.Tuners)
            {
                switch (tuner.Type)
                {
                    case TunerType.Cable:
                        var dvbcnode = new TreeNode("DVBC Tuner");
                        dvbcnode.Tag = "Cable";
                        servernode.Nodes.Add(dvbcnode);
                        break;
                    case TunerType.Satellite:
                        var dvbsnode = new TreeNode("DVBS Tuner");
                        dvbsnode.Tag = "Satellite";
                        servernode.Nodes.Add(dvbsnode);
                        break;
                    case TunerType.Terrestrial:
                        var dvbtnode = new TreeNode("DVBT Tuner");
                        dvbtnode.Tag = "Terrestrial";
                        servernode.Nodes.Add(dvbtnode);
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
            panel1.Controls.Clear();
            if(e.Node.Tag != null && e.Node.Tag is "Root")
            {
                var projectinfo = new ProjectInformation();
                panel1.Controls.Add(projectinfo);
                label1.Text = "Sat>Ip Project";
            }
            else if (e.Node.Tag != null && e.Node.Tag is SatIpDevice)
            {
                var device = ssdp.FindByUDN(e.Node.Name);
                if (device != null)
                {
                    var deviceinfo = new DeviceInformation(device);
                    label1.Text = e.Node.Text;
                    panel1.Controls.Add(deviceinfo);
                    //TransponderScan frm = new TransponderScan(device);
                    //frm.ShowDialog();
                }
            }
            else if (e.Node.Tag != null && e.Node.Tag is "Cable")
            {
                var device = ssdp.FindByUDN(e.Node.Parent.Name);
                var cabinfo = new CableInformation(device);
                panel1.Controls.Add(cabinfo);
                label1.Text = string.Format("{0} - {1}", device.FriendlyName, e.Node.Text);
            }
            else if (e.Node.Tag != null && e.Node.Tag is "Satellite")
            {
                var device = ssdp.FindByUDN(e.Node.Parent.Name);
                var satinfo = new SatelliteInformation(device);
                panel1.Controls.Add(satinfo);
                label1.Text = string.Format("{0} - {1}", device.FriendlyName, e.Node.Text);
            }
            else if (e.Node.Tag != null && e.Node.Tag is "Terrestrial")
            {
                var device = ssdp.FindByUDN(e.Node.Parent.Name);
                var terinfo = new TerrestrialInformation(device);
                panel1.Controls.Add(terinfo);
                label1.Text = string.Format("{0} - {1}", device.FriendlyName, e.Node.Text);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ssdp.Dispose();
            ssdp.DeviceFound -= new SSDPClient.DeviceFoundHandler(DeviceFound);
            ssdp.DeviceLost -= new SSDPClient.DeviceLostHandler(DeviceLost);            
            ssdp = null;
        }
    }
}
