/*  
    Copyright (C) <2007-2017>  <Kay Diefenthal>

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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SatIp
{
    public class UdpState
    {
        public UdpClient U;
        public IPEndPoint E;
    }

    public class SSDPClient : IDisposable
    {        
        private static readonly Regex UuidRegex = new Regex("(uuid:)(.+?)(?=(::)|$)");
        private static readonly Regex HttpResponseRegex = new Regex(@"HTTP/(\d+)\.(\d+)\s+(\d+)\s+([^.]+?)\r\n(.*)",
            RegexOptions.Singleline);

        private static readonly Regex MSearchResponseRegex = new Regex(@"M-SEARCH \* HTTP/(\d+)\.(\d+)\r\n(.*)",
            RegexOptions.Singleline);

        private static readonly Regex NotifyResponseRegex = new Regex(@"NOTIFY \* HTTP/(\d+)\.(\d+)\r\n(.*)",
            RegexOptions.Singleline);

        #region Private Fields

        private bool _running;
        private readonly string _multicastIp;
        private readonly int _multicastPort;
        private readonly int _unicastPort;
        private UdpClient _multicastClient;
        private UdpClient _unicastClient;
        private Dictionary<string, SatIpDevice> _devices = new Dictionary<string, SatIpDevice>();
        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a new instance of <see cref="SsdpClient"/> Class.
        /// It send SsdpReqeust(M-Search) and receives SsdpResponses(Http,M-Search,Notify) 
        /// </summary>
        public SSDPClient()
        {
            _multicastIp = "239.255.255.250";
            _multicastPort = 1900;
            _unicastPort = 1901;            
            _unicastClient = new UdpClient(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()),_unicastPort));
            _multicastClient = new UdpClient(_multicastPort);
            var ipSsdp = IPAddress.Parse(_multicastIp);
            _multicastClient.JoinMulticastGroup(ipSsdp);
            _running = true;
            UnicastSetBeginReceive();
            MulticastSetBeginReceive();
        }

        ~SSDPClient()
        {
            Dispose(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The MulticastReceiveCallback receives M-Search and Notify Responses
        /// M-Search Responses are ignored 
        /// Notify Responses fires SatIpDeviceLost Event if nts.Equals(ssdp:byebye) for removing the Device  
        /// or SatIpDeviceNotify Event if nts.Equals(ssdp:alive) for inform that the SatIp device is alive
        /// it can be used for initialize a new M-Seach Request too 
        /// </summary>
        /// <param name="ar"></param>
        private void MulticastReceiveCallback(IAsyncResult ar)
        {
            if (_running)
            {
                var u = ((UdpState)(ar.AsyncState)).U;
                var e = ((UdpState)(ar.AsyncState)).E;
                if (u.Client != null)
                {
                    var responseBytes = u.EndReceive(ar, ref e);
                    var responseString = Encoding.UTF8.GetString(responseBytes);
                    var msearchMatch = MSearchResponseRegex.Match(responseString);
                    if (msearchMatch.Success)
                    {
                        responseString = msearchMatch.Groups[3].Captures[0].Value;
                        Logger.Info("M-Search Response. \r\n\r\n{0} ", responseString);
                        var headerDictionary = Parse(responseString);
                        string host;
                        headerDictionary.TryGetValue("host", out host);
                        string man;
                        headerDictionary.TryGetValue("man", out man);
                        string mx;
                        headerDictionary.TryGetValue("mx", out mx);
                        string st;
                        headerDictionary.TryGetValue("st", out st);
                    }
                    var notifyMatch = NotifyResponseRegex.Match(responseString);
                    if (notifyMatch.Success)
                    {
                        responseString = notifyMatch.Groups[3].Captures[0].Value;
                        Logger.Info("Notify Response. \r\n\r\n{0} ", responseString);
                        var headerDictionary = Parse(responseString);
                        string location;
                        headerDictionary.TryGetValue("location", out location);
                        string host;
                        headerDictionary.TryGetValue("host", out host);
                        string nt;
                        headerDictionary.TryGetValue("nt", out nt);
                        string nts;
                        headerDictionary.TryGetValue("nts", out nts);
                        string usn;
                        headerDictionary.TryGetValue("usn", out usn);
                        string bootId;
                        headerDictionary.TryGetValue("bootid.upnp.org", out bootId);
                        string configId;
                        headerDictionary.TryGetValue("configid.upnp.org", out configId);
                        var m = UuidRegex.Match(usn);
                        if (!m.Success)
                            return;
                        var uuid = m.Value;
                        if (nts != null &&
                            (nt != null && (nt.Equals("urn:ses-com:device:SatIPServer:1") && nts.Equals("ssdp:byebye"))))
                        {
                            if (usn != null)
                            {
                                if (_devices.ContainsKey(uuid))
                                { _devices.Remove(uuid); }
                                OnDeviceLost(new SatIpDeviceLostArgs(uuid));
                            }
                        }
                        if (nts != null &&
                            (nt != null && (nt.Equals("urn:ses-com:device:SatIPServer:1") && nts.Equals("ssdp:alive"))))
                        {
                            if (!string.IsNullOrEmpty(location))
                            {
                                if (!_devices.ContainsKey(uuid))
                                {
                                    var device = new SatIpDevice(location);
                                    _devices.Add(uuid, device);
                                    OnDeviceFound(new SatIpDeviceFoundArgs(device));
                                }
                            }
                        }
                    }
                }
                MulticastSetBeginReceive();
            }            
        }

        /// <summary>
        /// Listen for Multicast SSDP Responses
        /// </summary>
        private void MulticastSetBeginReceive()
        {
            if (_running)
            {
                var ipSsdp = IPAddress.Parse(_multicastIp);
                var ipRxEnd = new IPEndPoint(ipSsdp, _multicastPort);
                UdpState udpListener = new UdpState { E = ipRxEnd };
                if (_multicastClient == null)
                    _multicastClient = new UdpClient(_multicastPort);
                udpListener.U = _multicastClient;
                _multicastClient.BeginReceive(MulticastReceiveCallback, udpListener);
            }
        }

        /// <summary>
        /// The UnicastReceiveCallback receives Http Responses 
        /// and Fired the SatIpDeviceFound Event for adding the SatIpDevice  
        /// </summary>
        /// <param name="ar"></param>
        private void UnicastReceiveCallback(IAsyncResult ar)
        {
            if(_running)
            {
                var u = ((UdpState) (ar.AsyncState)).U;
                var e = ((UdpState) (ar.AsyncState)).E;
                if (u.Client != null)
                {
                    var responseBytes = u.EndReceive(ar, ref e);
                    var responseString = Encoding.UTF8.GetString(responseBytes);
                    var httpMatch = HttpResponseRegex.Match(responseString);
                    if (httpMatch.Success)
                    {
                        responseString = httpMatch.Groups[5].Captures[0].Value;
                        Logger.Info("Http Response.\r\n\r\n{0} ", responseString);
                        var headerDictionary = Parse(responseString);
                        string location;
                        headerDictionary.TryGetValue("location", out location);
                        string st;
                        headerDictionary.TryGetValue("st", out st);
                        string usn;
                        headerDictionary.TryGetValue("usn", out usn);
                        string bootId;
                        headerDictionary.TryGetValue("bootid.upnp.org", out bootId);
                        string configId;
                        headerDictionary.TryGetValue("configid.upnp.org", out configId);
                        string deviceId;
                        headerDictionary.TryGetValue("deviceid.ses.com", out deviceId);
                        var m = UuidRegex.Match(usn);
                        if (!m.Success)
                            return;
                        var uuid = m.Value;
                        if ((!string.IsNullOrEmpty(location)) && (!string.IsNullOrEmpty(st)) && (st.Equals("urn:ses-com:device:SatIPServer:1")))
                        {
                            if (!_devices.ContainsKey(uuid))
                            {
                                var device = new SatIpDevice(location);
                                _devices.Add(uuid, device);
                                OnDeviceFound(new SatIpDeviceFoundArgs(device));
                            }
                        }
                    }                    
                }
                UnicastSetBeginReceive();                    
            }
        }

        public static Dictionary<string, string> Parse(string searchResponse)
        {
            var reader = new StringReader(searchResponse);
            var line = string.Empty;
            var values = new Dictionary<string, string>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    continue;
                }
                var colon = line.IndexOf(':');
                if (colon < 1)
                {
                    return null;
                }
                var name = line.Substring(0, colon).Trim();
                var value = line.Substring(colon + 1).Trim();
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                values[name.ToLowerInvariant()] = value;
            }
            return values;
        }

        /// <summary>
        /// Listen for Unicast SSDP Responses
        /// </summary>
        private void UnicastSetBeginReceive()
        {
            if (_running)
            {
                var ipRxEnd = new IPEndPoint(IPAddress.Any, _unicastPort);
                var udpListener = new UdpState { E = ipRxEnd };
                if (_unicastClient == null)
                    _unicastClient = new UdpClient(_unicastPort);
                udpListener.U = _unicastClient;
                _unicastClient.BeginReceive(UnicastReceiveCallback, udpListener);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends SsdpRequest M-SEARCH 
        /// </summary>
        /// <param name="searchterm"></param>
        public void FindByType(string searchterm )
        {
            var query = new StringBuilder();
            query.Append("M-SEARCH * HTTP/1.1\r\n");
            query.Append("HOST: 239.255.255.250:1900\r\n");
            query.Append("MAN: \"ssdp:discover\"\r\n");
            query.Append("MX: 2\r\n");
            query.Append("ST: " + searchterm + "\r\n");
            query.Append("\r\n");
            if (_unicastClient == null)
            {
                _unicastClient = new UdpClient(_unicastPort);
            }
            byte[] req = Encoding.UTF8.GetBytes(query.ToString());
            var ipSsdp = IPAddress.Parse(_multicastIp);
            var ipTxEnd = new IPEndPoint(ipSsdp, _multicastPort);
            for (var i = 0; i < 2; i++)
            {
                if (i > 0)
                    Thread.Sleep(33);
                _unicastClient.Send(req, req.Length, ipTxEnd);
            }
        }
        public SatIpDevice FindByUDN(string uuid)
        {
            SatIpDevice device = null;
            if (_devices.ContainsKey(uuid))
            {
                _devices.TryGetValue(uuid, out device);
            }
            return device;
        }
        /// <summary>
        /// Start Unicast and Multicast Listener
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            _unicastClient = new UdpClient(_unicastPort);
            _multicastClient = new UdpClient(_multicastPort);
            var ipSsdp = IPAddress.Parse(_multicastIp);
            _multicastClient.JoinMulticastGroup(ipSsdp);
            _running = true;
            UnicastSetBeginReceive();
            MulticastSetBeginReceive();
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region  Protected Methods

        protected void OnDeviceFound(SatIpDeviceFoundArgs args)
        {
            if (DeviceFound != null)
            {
                DeviceFound(this, args);
            }
        }

        protected void OnDeviceLost(SatIpDeviceLostArgs args)
        {
            if (DeviceLost != null)
            {
                DeviceLost(this, args);
            }
        }        

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _running = false;
                    _multicastClient.Close();
                    _unicastClient.Close();
                }
            }
            _disposed = true;
        }

        #endregion

        #region Properties

        public bool IsRunning()
        {
            return _running;
        }

        #endregion

        #region  Delegates

        public delegate void DeviceFoundHandler(object sender, SatIpDeviceFoundArgs e);

        public delegate void DeviceLostHandler(object sender, SatIpDeviceLostArgs e);

        //public delegate void DeviceNotifyHandler(object sender, SatIpDeviceNotifyArgs e);

        #endregion

        #region Public Events

        public event DeviceFoundHandler DeviceFound;
        public event DeviceLostHandler DeviceLost;        
        private bool _disposed;

        #endregion
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

    }
    public class SatIpDeviceFoundArgs : EventArgs
    {
        public SatIpDevice Device { get; private set; }

        public SatIpDeviceFoundArgs(SatIpDevice device)
        {
            Device = device;
        }
    }   

    /// <summary>
    /// 
    /// </summary>
    public class SatIpDeviceLostArgs : EventArgs
    {
        public String Uuid { get; private set; }

        public SatIpDeviceLostArgs(string uuid)
        {
            Uuid = uuid;
        }
    }
}

