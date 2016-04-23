using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using SatIp.Analyzer;
using SatIp.Scan.Properties;

namespace SatIp
{
    
    public partial class Form2 : Form
    {
        private SatIpDevice _device;        
        private bool _isScanning = false;
        private bool _stopScanning=false;
        string _file;
        
        private AutoResetEvent _keepAliveThreadStopEvent = null;
        private AutoResetEvent _scanThreadStopEvent = null;
        private Thread _scanThread;
        private Thread _keepAliveThread;
        private AutoResetEvent _rtpThreadStopEvent = null;
        private Thread _rtpThread;
        private IPEndPoint _remoteEndPoint;
        private UdpClient _udpclient;
        BinaryWriter _binWriter;
        public Form2(SatIpDevice device)
        {
            InitializeComponent();
            _device = device;
            #region DeviceInfo
            tbxDeviceType.Text = device.DeviceType;
            tbxFriendlyName.Text = device.FriendlyName;
            tbxManufacture.Text = device.Manufacturer;
            tbxModelDescription.Text = device.ModelDescription;
            tbxUniqueDeviceName.Text = device.UniqueDeviceName;
            pbxDVBC.Image = Resources.dvb_c;
            pbxDVBC.Visible = device.SupportsDVBC;
            pbxDVBS.Image = Resources.dvb_s;
            pbxDVBS.Visible = device.SupportsDVBS;
            pbxDVBT.Image = Resources.dvb_t;
            pbxDVBT.Visible = device.SupportsDVBT;

            try
            {
                string imageUrl = string.Format(device.FriendlyName == "OctopusNet" ? "http://{0}:{1}/{2}" : "http://{0}:{1}{2}", device.BaseUrl.Host, device.BaseUrl.Port, device.GetImage(1));
                pbxManufactureBrand.LoadAsync(imageUrl);
                pbxManufactureBrand.Visible = true;
            }
            catch
            {
                pbxManufactureBrand.Visible = false;
            }

            #endregion
        }
        private void UpdateSatelliteSettings()
        {
            base.SuspendLayout();
            if (cbxDiseqC.SelectedIndex == 0)
            {
                lblSourceB.Visible = false;
                cbxSourceB.Visible = false;                
            }
            else
            {
                lblSourceB.Visible = true;
                cbxSourceB.Visible = true;
            }
            if ((cbxDiseqC.SelectedIndex == 0) || (cbxDiseqC.SelectedIndex == 1))
            {               
                lblSourceC.Visible = false;
                cbxSourceC.Visible = false;
                lblSourceD.Visible = false;
                cbxSourceD.Visible = false;
            }
            else
            {                
                lblSourceC.Visible = true;
                cbxSourceC.Visible = true;
                lblSourceD.Visible = true;
                cbxSourceD.Visible = true;
            }
            base.ResumeLayout();
        }

        private void cbxDiseqC_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSatelliteSettings();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
            #region DVBSSources

            cbxDiseqC.Items.Add("None(Single Lnb)");
            cbxDiseqC.Items.Add("22 KHz (Tone Switch)");
            cbxDiseqC.Items.Add("Diseq c 1.x (A/B/C/D");
            cbxDiseqC.SelectedIndex = 0;

            var app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var tuningdata = app + "\\TuningData\\Satellite";
            cbxSourceA.Items.Add("- None -");
            cbxSourceA.SelectedIndex = 0;
            cbxSourceB.Items.Add("- None -");
            cbxSourceB.SelectedIndex = 0;
            cbxSourceC.Items.Add("- None -");
            cbxSourceC.SelectedIndex = 0;
            cbxSourceD.Items.Add("- None -");
            cbxSourceD.SelectedIndex = 0;
            foreach (var str2 in Directory.GetFiles(tuningdata))
            {
                IniReader reader = new IniReader(str2);
                var str3 = reader.ReadString("SATTYPE", "1");
                var str4 = reader.ReadString("SATTYPE", "2");
                if (!cbxSourceA.Items.Contains(str4) && (str4 != ""))
                {
                    cbxSourceA.Items.Add(new IniMapping(str3 + " " + str4, str2));
                    cbxSourceB.Items.Add(new IniMapping(str3 + " " + str4, str2));
                    cbxSourceC.Items.Add(new IniMapping(str3 + " " + str4, str2));
                    cbxSourceD.Items.Add(new IniMapping(str3 + " " + str4, str2));
                }
            }
            UpdateSatelliteSettings(); 
            #endregion
        }

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
       

       
        private void Scan(string tuning)
        {
            ProgramAssociationTable pat;
            ProgramMapTable pmt;
            Dictionary<int, ProgramMapTable> pmts = new Dictionary<int,ProgramMapTable>();
            ServiceDescriptionTable sdt;

            bool signalLocked;
            int signallevel;
            int signalQuality;
            RtspStatusCode statuscode;
            if (string.IsNullOrEmpty(_device.RtspSession.RtspSessionId))
            {                
                statuscode =_device.RtspSession.Setup(tuning, "unicast");
                if (statuscode.Equals(RtspStatusCode.Ok))
                {
                    StartKeepAliveThread();

                    _udpclient = new UdpClient(40000);
                    _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                }
                else
                {
                    MessageBox.Show(String.Format("Setup retuns {0}", statuscode), "Failure", MessageBoxButtons.OK);
                }
            }
            else
            {
                _device.RtspSession.Play(tuning);
            }            
            statuscode = _device.RtspSession.Describe(out signalLocked, out signallevel, out signalQuality);
            if (!statuscode.Equals(RtspStatusCode.Ok))
            {
                MessageBox.Show(String.Format("Describe retuns {0}", statuscode), "Failure", MessageBoxButtons.OK);
            }
            SetControlPropertyThreadSafe(pgbSignalLevel, "Value", signallevel);
            SetControlPropertyThreadSafe(pgbSignalQuality, "Value", signalQuality);                        
            if (signalLocked)
            {               
                //Say the Sat>IP server we want Receives the ProgramAssociationTable
                _device.RtspSession.Play("addpids=0");
                GetPAT(_udpclient, _remoteEndPoint, out pat);
                Console.Write(pat.ToString());
                //Say the Sat>IP server we want not more Receives the ProgramAssociationTable
                _device.RtspSession.Play(string.Format("delpids=0"));
                //// Loop the ProgramAssociationTable Programs
                foreach (var program in pat.Programs)
                {
                    //Say the Sat>IP server we want Receives the ProgramMapTable for Pid x                            
                    _device.RtspSession.Play(string.Format("addpids={0}", program.Pid));
                    GetPMT(_udpclient, _remoteEndPoint, program.Pid, out pmt);
                    Console.Write(pmt.ToString());
                    // Add the ProgramMapTable for Pid x into the Dictionary
                    pmts.Add(pmt.ProgramNumber, pmt);
                    // Say the Sat>IP server we want not more Receives the ProgramMapTable for Pid x
                    _device.RtspSession.Play(string.Format("delpids={0}", program.Pid));
                }                    
                // Say the Sat>IP server we want Receives the ServiceDescriptionTable 
                _device.RtspSession.Play("addpids=17");
                GetSDT(_udpclient, _remoteEndPoint, out sdt);                
                Console.Write(sdt.ToString());
                // Say the Sat>IP server we want not more Receives the ServiceDescriptionTable
                _device.RtspSession.Play(string.Format("delpids=17"));
                
                    
                
                // Now we had we all to create an service 
            }
        }

        private bool GetPAT(UdpClient client, IPEndPoint endpoint, out ProgramAssociationTable programAssociationTable)
        {
            ProgramAssociationTable pat = new ProgramAssociationTable();
            int currentVersion = -1;
            int nextSectionNumber = 0;
            bool retval = false;
            while (!retval)
            {
                var receivedbytes = client.Receive(ref endpoint);
                if ((receivedbytes.Length > 12) && ((receivedbytes.Length - 12) % 188) == 0)
                {
                    double num9 = (((double)(receivedbytes.Length - 12)) / 188.0) - 1.0;
                    for (double j = 0.0; j <= num9; j++)
                    {
                        byte[] destinationarray = (byte[])Array.CreateInstance(typeof(byte), 188);
                        Array.Copy(receivedbytes, (int)Math.Round((double)(12.0 + (j * 188))), destinationarray, 0, 188);
                        if (destinationarray[0] == 0x47 && destinationarray.Length >= 188 && (destinationarray[1] & 15) == 0 && destinationarray[2] == 0x00 && destinationarray[5] == 0x00)
                        {
                            //Reads the TsHeader
                            var tsheader = TsHeader.Decode(destinationarray);
                            //Decode the Table 
                            pat = ProgramAssociationTable.Decode(tsheader.PayloadUnitStartIndicator, tsheader.PayLoadStart, destinationarray);
                            //nextSectionNumber++;
                            //if (nextSectionNumber >= pat.LastSectionNumber)
                            //{
                                programAssociationTable = pat;                                
                                retval= true;
                            //}
                            //programAssociationTable = null;
                            //retval = false;
                        }
                    }
                }                
            }            
            programAssociationTable = pat;
            return retval;
        }
        private bool GetPMT(UdpClient client, IPEndPoint endpoint, int pid,out ProgramMapTable programMapTable)
        {            
            ProgramMapTable pmt = new ProgramMapTable();
            int currentVersion = -1;
            int nextSectionNumber = 0;
            bool retval = false;
            while (!retval)
            {
                var receivedbytes = client.Receive(ref endpoint);
                if ((receivedbytes.Length > 12) && ((receivedbytes.Length - 12) % 188) == 0)
                {
                    double num9 = (((double)(receivedbytes.Length - 12)) / 188.0) - 1.0;
                    for (double j = 0.0; j <= num9; j++)
                    {
                        byte[] destinationarray = (byte[])Array.CreateInstance(typeof(byte), 188);
                        Array.Copy(receivedbytes, (int)Math.Round((double)(12.0 + (j * 188))), destinationarray, 0, 188);
                        if ((destinationarray[0] == 0x47 && destinationarray.Length >= 188 && destinationarray[2] == pid && destinationarray[5] == 0x02))
                        {
                            //Reads the TsHeader
                            var tstheader = TsHeader.Decode(destinationarray);
                            //Decode the Table                            
                            pmt = ProgramMapTable.Decode(tstheader.PayloadUnitStartIndicator, tstheader.PayLoadStart, destinationarray);
                            nextSectionNumber++;
                            if (nextSectionNumber >= pmt.LastSectionNumber)
                            {
                                programMapTable = pmt;
                                retval= true;
                            }
                            //programMapTable = null;
                            //retval = false;
                        }
                    }
                }
                //programMapTable = null;
                //retval= false;
            }
            programMapTable = pmt;
            return retval;
        }
        private bool GetSDT(UdpClient client, IPEndPoint endpoint,out ServiceDescriptionTable serviceDescriptionTable)
        {
            ServiceDescriptionTable sdt = new ServiceDescriptionTable();
            int currentVersion = -1;
            int nextSectionNumber = 0;
            bool retval = false;
            while (!retval)
            {                
                var receivedbytes = client.Receive(ref endpoint);
                if ((receivedbytes.Length > 12) && ((receivedbytes.Length - 12) % 188) == 0)
                {
                    double num9 = (((double)(receivedbytes.Length - 12)) / 188.0) - 1.0;
                    for (double j = 0.0; j <= num9; j++)
                    {
                        byte[] destinationarray = (byte[])Array.CreateInstance(typeof(byte), 188);
                        Array.Copy(receivedbytes, (int)Math.Round((double)(12.0 + (j * 188))), destinationarray, 0, 188);
                        if (destinationarray[0] == 0x47 && destinationarray.Length >= 188 && (destinationarray[1] & 15) == 0 && destinationarray[2] == 0x11 && destinationarray[5] == 0x42)                                       	//check if it is a SDT (Service Description Table) destinationarray?
                        {
                            //Reads the TsHeader
                            var tsheader = TsHeader.Decode(destinationarray);
                            //Decode the Table                        
                            sdt = ServiceDescriptionTable.Decode(tsheader.PayloadUnitStartIndicator, tsheader.PayLoadStart, destinationarray);
                            //nextSectionNumber++;
                            //if (nextSectionNumber >= sdt.LastSectionNumber)
                            //{
                                serviceDescriptionTable = sdt;
                                retval = true;
                            //}
                            //serviceDescriptionTable = null;
                            //retval = false; 
                        }
                    }
                }
                //serviceDescriptionTable = null;
                //retval = false;
            }
            serviceDescriptionTable = sdt;
            return retval;
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_device!= null)
            {
                _device.Dispose();
            }
        }

        private void StartKeepAliveThread()
        {            
            if (_keepAliveThread != null && !_keepAliveThread.IsAlive)
            {
                StopKeepAliveThread();
            }

            if (_keepAliveThread == null)
            {                
                _keepAliveThreadStopEvent = new AutoResetEvent(false);
                _keepAliveThread = new Thread(new ThreadStart(KeepAlive));
                _keepAliveThread.Name="SAT>IP tuner streaming keep-alive";
                _keepAliveThread.IsBackground = true;
                _keepAliveThread.Priority = ThreadPriority.Lowest;
                _keepAliveThread.Start();
            }
        }
        private void StopKeepAliveThread()
        {
            if (_keepAliveThread != null)
            {
                if (!_keepAliveThread.IsAlive)
                {
                    //this.LogWarn("SAT>IP base: aborting old streaming keep-alive thread");
                    _keepAliveThread.Abort();
                }
                else
                {
                    _keepAliveThreadStopEvent.Set();
                    if (!_keepAliveThread.Join(_device.RtspSession.RtspSessionTimeToLive))
                    {
                        //this.LogWarn("SAT>IP base: failed to join streaming keep-alive thread, aborting thread");
                        _keepAliveThread.Abort();
                    }
                }
                _keepAliveThread = null;
                if (_keepAliveThreadStopEvent != null)
                {
                    _keepAliveThreadStopEvent.Close();
                    _keepAliveThreadStopEvent = null;
                }
            }
        }
        private void KeepAlive()
        {
            try
            {
                while (!_keepAliveThreadStopEvent.WaitOne((_device.RtspSession.RtspSessionTimeToLive) ))  // -5 seconds to avoid timeout
                {
                    _device.RtspSession.Options();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                //this.LogError(ex, "SAT>IP base: streaming keep-alive thread exception");
                return;
            }
            //this.LogDebug("SAT>IP base: streaming keep-alive thread stopping");
        }
        private void StartScanThread()
        {
            // Kill the existing thread if it is in "zombie" state.
            if (_scanThread != null && !_scanThread.IsAlive)
            {
                StopScanThread();
            }

            if (_scanThread == null)
            {
                //this.LogDebug("SAT>IP base: starting new streaming keep-alive thread");
                _scanThreadStopEvent = new AutoResetEvent(false);
                _scanThread = new Thread(new ThreadStart(DoScan));
                _scanThread.Name = "SAT>IP Scan";
                _scanThread.IsBackground = true;
                _scanThread.Priority = ThreadPriority.Normal;
                _scanThread.Start();
            }
        }
        private void StopScanThread()
        {
            if (_scanThread != null)
            {
                if (!_scanThread.IsAlive)
                {
                    //this.LogWarn("SAT>IP base: aborting old streaming keep-alive thread");
                    _scanThread.Abort();
                }
                else
                {
                    _scanThreadStopEvent.Set();                    
                }
                _scanThread = null;
                if (_scanThreadStopEvent != null)
                {
                    _scanThreadStopEvent.Close();
                    _scanThreadStopEvent = null;
                }
            }
        }
        private void DoScan()
        {
            _isScanning = true;
            _stopScanning = false;
            SetControlPropertyThreadSafe(btnScan, "Text", "Stop Search");
            IniReader reader = new IniReader(_file);
            var Count = reader.ReadInteger("DVB", "0", 0);
            try
            {
                var Index = 1;
                string source = "1";
                string tuning;
                while (Count > Index)
                {
                    if (_stopScanning) return;
                    float percent = ((float)(Index)) / Count;
                    percent *= 100f;
                    if (percent > 100f) percent = 100f;
                    SetControlPropertyThreadSafe(pgbSearchResult, "Value", (int)percent);
                    string[] strArray = reader.ReadString("DVB", Index.ToString()).Split(new char[] { ',' });

                    if (strArray[4] == "S2")
                    {
                        tuning = string.Format("src={0}&freq={1}&pol={2}&sr={3}&fec={4}&msys=dvbs2&mtype={5}&plts=on&ro=0.35&pids=0", source, strArray[0].ToString(), strArray[1].ToLower().ToString(), strArray[2].ToLower().ToString(), strArray[3].ToString(), strArray[5].ToLower().ToString());
                    }
                    else
                    {
                        tuning = string.Format("src={0}&freq={1}&pol={2}&sr={3}&fec={4}&msys=dvbs&mtype={5}&pids=0", source, strArray[0].ToString(), strArray[1].ToLower().ToString(), strArray[2].ToString(), strArray[3].ToString(), strArray[5].ToLower().ToString());
                    }
                    Scan(tuning);
                    //Thread.Sleep(1000);
                    Index++;
                    //ListViewItem item = lwResults.Items.Add(new ListViewItem(strArray[0].ToString()));
                    //item.EnsureVisible();                    
                }
                //lwResults.Items.Add(new ListViewItem(String.Format("Total radio channels new:{0} updated:{1}", 1, 1)));
                //lwResults.Items.Add(new ListViewItem(String.Format("Total tv channels new:{0} updated:{1}", 1, 1)));
                //ListViewItem itm = lwResults.Items.Add(new ListViewItem("Scan done..."));
                //itm.EnsureVisible();
            }
            catch
            {
            }
            finally
            {
                _udpclient.Close();
                StopKeepAliveThread();
                _device.RtspSession.TearDown();
                SetControlPropertyThreadSafe(pgbSearchResult, "Value", 100);

                _isScanning = false;
                SetControlPropertyThreadSafe(btnScan, "Text", "Start Search");
                StopScanThread();
            }
        }
        private void StartRtpThread()
        {
            
            _binWriter = new BinaryWriter(new MemoryStream());
            // Kill the existing thread if it is in "zombie" state.
            if (_rtpThread != null && !_rtpThread.IsAlive)
            {
                StopRtpThread();
            }

            if (_rtpThread == null)
            {
                //this.LogDebug("SAT>IP base: starting new streaming keep-alive thread");
                _rtpThreadStopEvent = new AutoResetEvent(false);
                _rtpThread = new Thread(new ThreadStart(DoRtp));
                _rtpThread.Name = "SAT>IP RTP";
                _rtpThread.IsBackground = true;
                _rtpThread.Priority = ThreadPriority.Lowest;
                _rtpThread.Start();
            }
        }
        private void StopRtpThread()
        {
            if (_rtpThread != null)
            {
                if (!_rtpThread.IsAlive)
                {
                    //this.LogWarn("SAT>IP base: aborting old streaming keep-alive thread");
                    _rtpThread.Abort();
                }
                else
                {
                    _rtpThreadStopEvent.Set();
                }
                _rtpThread = null;
                if (_rtpThreadStopEvent != null)
                {
                    _rtpThreadStopEvent.Close();
                    _rtpThreadStopEvent = null;
                }
            }
        }
        private void DoRtp()
        {
            
            try
            {
                while (!_rtpThreadStopEvent.WaitOne(1))
                {
                    byte[] receivedbytes = _udpclient.Receive(ref _remoteEndPoint);
                    //var packet = RtpHeader.Decode(receivedbytes);                    
                    //Console.Write(packet.ToString() + "\r\n");
                    if ((receivedbytes.Length > 12) && ((receivedbytes.Length - 12) % 188) == 0)
                    {
                        double num9 = (((double)(receivedbytes.Length - 12)) / 188.0) - 1.0;
                        for (double j = 0.0; j <= num9; j++)
                        {
                            byte[] destinationarray = (byte[])Array.CreateInstance(typeof(byte), 188);
                            Array.Copy(receivedbytes, (int)Math.Round((double)(12.0 + (j * 188))), destinationarray, 0, 188);
                            if ((destinationarray[0] == 0x47 && destinationarray.Length >= 188))
                            {
                                _binWriter.Write(destinationarray);
                            }
                        }
                    }
                    //_receivedBytes += packets.Length;
                    //_receivedPackets++;
                }
            }
            finally
            {
                _udpclient.Close();
            }

            
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            var ListA = (IniMapping)cbxSourceA.SelectedItem;
            _file = ListA.File;
            if (_isScanning == false)
            {

                StartScanThread();
            }
            else
            {
                _stopScanning = true;
            }
        }
    }
    
}
