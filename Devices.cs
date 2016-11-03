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
        private bool _isScanning = false;
        private bool _stopScanning = false;
        string _file;
        private AutoResetEvent _scanThreadStopEvent = null;
        private Thread _scanThread;
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

        //private void UpdateSatelliteSettings()
        //{
        //    base.SuspendLayout();
        //    if (cbxDiseqC.SelectedIndex == 0)
        //    {
        //        lblSourceB.Visible = false;
        //        cbxSourceB.Visible = false;
        //    }
        //    else
        //    {
        //        lblSourceB.Visible = true;
        //        cbxSourceB.Visible = true;
        //    }
        //    if ((cbxDiseqC.SelectedIndex == 0) || (cbxDiseqC.SelectedIndex == 1))
        //    {
        //        lblSourceC.Visible = false;
        //        cbxSourceC.Visible = false;
        //        lblSourceD.Visible = false;
        //        cbxSourceD.Visible = false;
        //    }
        //    else
        //    {
        //        lblSourceC.Visible = true;
        //        cbxSourceC.Visible = true;
        //        lblSourceD.Visible = true;
        //        cbxSourceD.Visible = true;
        //    }
        //    base.ResumeLayout();
        //}

        //private void cbxDiseqC_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    UpdateSatelliteSettings();
        //}

        //private void DevicesForm_Load(object sender, EventArgs e)
        //{
        //    #region Sources
        //    cbxDiseqC.Items.Add("None(Single Lnb)");
        //    cbxDiseqC.Items.Add("22 KHz (Tone Switch)");
        //    cbxDiseqC.Items.Add("Diseq c 1.x (A/B/C/D");
        //    cbxDiseqC.SelectedIndex = 0;

        //    var app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    var tuningdata = app + "\\TuningData\\Satellite";
        //    cbxSourceA.Items.Add("- None -");
        //    cbxSourceA.SelectedIndex = 0;
        //    cbxSourceB.Items.Add("- None -");
        //    cbxSourceB.SelectedIndex = 0;
        //    cbxSourceC.Items.Add("- None -");
        //    cbxSourceC.SelectedIndex = 0;
        //    cbxSourceD.Items.Add("- None -");
        //    cbxSourceD.SelectedIndex = 0;
        //    foreach (var str2 in Directory.GetFiles(tuningdata))
        //    {
        //        IniReader reader = new IniReader(str2);
        //        var str3 = reader.ReadString("SATTYPE", "1");
        //        var str4 = reader.ReadString("SATTYPE", "2");
        //        if (!cbxSourceA.Items.Contains(str4) && (str4 != ""))
        //        {
        //            cbxSourceA.Items.Add(new IniMapping(str3 + " " + str4, str2));
        //            cbxSourceB.Items.Add(new IniMapping(str3 + " " + str4, str2));
        //            cbxSourceC.Items.Add(new IniMapping(str3 + " " + str4, str2));
        //            cbxSourceD.Items.Add(new IniMapping(str3 + " " + str4, str2));
        //        }
        //    }
        //    UpdateSatelliteSettings();
        //    #endregion
        //}
        //private void btnScan_Click(object sender, EventArgs e)
        //{
        //    var ListA = (IniMapping)cbxSourceA.SelectedItem;
        //    _file = ListA.File;
        //    if (_isScanning == false)
        //    {

        //        StartScanThread();
        //    }
        //    else
        //    {
        //        _stopScanning = true;
        //    }
        //}
        //private void StartScanThread()
        //{
        //    // Kill the existing thread if it is in "zombie" state.
        //    if (_scanThread != null && !_scanThread.IsAlive)
        //    {
        //        StopScanThread();
        //    }

        //    if (_scanThread == null)
        //    {
        //        //this.LogDebug("SAT>IP base: starting new streaming keep-alive thread");
        //        _scanThreadStopEvent = new AutoResetEvent(false);
        //        _scanThread = new Thread(new ThreadStart(DoScan));
        //        _scanThread.Name = "SAT>IP Scan";
        //        _scanThread.IsBackground = true;
        //        _scanThread.Priority = ThreadPriority.Lowest;
        //        _scanThread.Start();
        //    }
        //}
        //private void StopScanThread()
        //{
        //    if (_scanThread != null)
        //    {
        //        if (!_scanThread.IsAlive)
        //        {
        //            //this.LogWarn("SAT>IP base: aborting old streaming keep-alive thread");
        //            _scanThread.Abort();
        //        }
        //        else
        //        {
        //            _scanThreadStopEvent.Set();
        //        }
        //        _scanThread = null;
        //        if (_scanThreadStopEvent != null)
        //        {
        //            _scanThreadStopEvent.Close();
        //            _scanThreadStopEvent = null;
        //        }
        //    }
        //}
        //private void DoScan()
        //{
        //    _isScanning = true;
        //    _stopScanning = false;
        //    SetControlPropertyThreadSafe(btnScan, "Text", "Stop Search");
        //    IniReader reader = new IniReader(_file);
        //    var Count = reader.ReadInteger("DVB", "0", 0);
        //    try
        //    {
        //        var Index = 1;
        //        string source = "1";
        //        string tuning;
        //        while (Count > Index)
        //        {
        //            if (_stopScanning) return;
        //            float percent = ((float)(Index)) / Count;
        //            percent *= 100f;
        //            if (percent > 100f) percent = 100f;
        //            SetControlPropertyThreadSafe(pgbSearchResult, "Value", (int)percent);
        //            string[] strArray = reader.ReadString("DVB", Index.ToString()).Split(new char[] { ',' });

        //            if (strArray[4] == "S2")
        //            {
        //                tuning = string.Format("src={0}&freq={1}&pol={2}&sr={3}&fec={4}&msys=dvbs2&mtype={5}&plts=on&ro=0.35&pids=0", source, strArray[0].ToString(), strArray[1].ToLower().ToString(), strArray[2].ToLower().ToString(), strArray[3].ToString(), strArray[5].ToLower().ToString());
        //            }
        //            else
        //            {
        //                tuning = string.Format("src={0}&freq={1}&pol={2}&sr={3}&fec={4}&msys=dvbs&mtype={5}&pids=0", source, strArray[0].ToString(), strArray[1].ToLower().ToString(), strArray[2].ToString(), strArray[3].ToString(), strArray[5].ToLower().ToString());
        //            }
        //            //Scan(tuning);
        //            Thread.Sleep(500);
        //            Index++;
        //            //ListViewItem item = lwResults.Items.Add(new ListViewItem(strArray[0].ToString()));
        //            //item.EnsureVisible();                    
        //        }
        //        //lwResults.Items.Add(new ListViewItem(String.Format("Total radio channels new:{0} updated:{1}", 1, 1)));
        //        //lwResults.Items.Add(new ListViewItem(String.Format("Total tv channels new:{0} updated:{1}", 1, 1)));
        //        //ListViewItem itm = lwResults.Items.Add(new ListViewItem("Scan done..."));
        //        //itm.EnsureVisible();
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        //_udpclient.Close();
        //        //StopKeepAliveThread();
        //        //_device.RtspSession.TearDown();
        //        SetControlPropertyThreadSafe(pgbSearchResult, "Value", 100);

        //        _isScanning = false;
        //        SetControlPropertyThreadSafe(btnScan, "Text", "Start Search");
        //        StopScanThread();
        //    }
        //}
        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        private static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                (SetControlPropertyThreadSafe),
                new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new object[] { propertyValue });
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
