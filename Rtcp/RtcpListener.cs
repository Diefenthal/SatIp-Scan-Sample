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
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace SatIp
{
    public class RtcpListener :IDisposable
    {
        private Thread _rtcpListenerThread;
        private AutoResetEvent _rtcpListenerThreadStopEvent = null;
        private UdpClient _udpClient;
        private IPEndPoint _multicastEndPoint;
        private IPEndPoint _serverEndPoint;
        private TransmissionMode _transmissionMode;

        public RtcpListener(String address, int port, TransmissionMode mode)
        {
            _transmissionMode = mode;
            switch (mode)
            {
                case TransmissionMode.Unicast:
                    _udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(address), port));
                    _serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    break;
                case TransmissionMode.Multicast:
                    _multicastEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
                    _serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    _udpClient = new UdpClient();
                    _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    _udpClient.ExclusiveAddressUse = false;
                    _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
                    _udpClient.JoinMulticastGroup(_multicastEndPoint.Address);
                    break;
            }            
        }
        ~RtcpListener()
        {
            Dispose(false);
        }   
        public void StartRtcpListenerThread()
        {            
            if (_rtcpListenerThread != null && !_rtcpListenerThread.IsAlive)
            {
                StopRtcpListenerThread();
            }

            if (_rtcpListenerThread == null)
            {
                Logger.Info("SAT>IP : starting new RTCP listener thread");
                _rtcpListenerThreadStopEvent = new AutoResetEvent(false);
                _rtcpListenerThread = new Thread(new ThreadStart(RtcpListenerThread));
                _rtcpListenerThread.Name = string.Format("SAT>IP tuner  RTCP listener");
                _rtcpListenerThread.IsBackground = true;
                _rtcpListenerThread.Priority = ThreadPriority.Lowest;
                _rtcpListenerThread.Start();
            }
        }

        public void StopRtcpListenerThread()
        {
            if (_rtcpListenerThread != null)
            {
                if (!_rtcpListenerThread.IsAlive)
                {
                    Logger.Warn("SAT>IP : aborting old RTCP listener thread");
                    _rtcpListenerThread.Abort();
                }
                else
                {
                    _rtcpListenerThreadStopEvent.Set();
                    if (!_rtcpListenerThread.Join(400 * 2))
                    {
                        Logger.Warn("SAT>IP : failed to join RTCP listener thread, aborting thread");
                        _rtcpListenerThread.Abort();
                    }
                }
                _rtcpListenerThread = null;
                if (_rtcpListenerThreadStopEvent != null)
                {
                    _rtcpListenerThreadStopEvent.Close();
                    _rtcpListenerThreadStopEvent = null;
                }
            }
        }

        private void RtcpListenerThread()
        {
            try
            {               
                bool receivedGoodBye = false;                
                try
                {
                    //_udpClient.Client.ReceiveTimeout = 400;
                    IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    while (!receivedGoodBye && !_rtcpListenerThreadStopEvent.WaitOne(1))
                    {
                        byte[] packets = _udpClient.Receive(ref serverEndPoint);
                        if (packets == null)
                        {
                            continue;
                        }
                        
                        int offset = 0;
                        while (offset < packets.Length)
                        {
                            switch (packets[offset + 1])
                            {
                                case 200: //sr
                                    var sr = new RtcpSenderReportPacket();
                                    sr.Parse(packets, offset);                                    
                                    offset += sr.Length;
                                    break;
                                case 201: //rr
                                    var rr = new RtcpReceiverReportPacket();
                                    rr.Parse(packets, offset);                                    
                                    offset += rr.Length;
                                    break;
                                case 202: //sd
                                    var sd = new RtcpSourceDescriptionPacket();
                                    sd.Parse(packets, offset);                                    
                                    offset += sd.Length;
                                    break;
                                case 203: // bye
                                    var bye = new RtcpByePacket();
                                    bye.Parse(packets, offset);                                    
                                    receivedGoodBye = true;
                                    OnPacketReceived(new RtcpPacketReceivedArgs(bye));
                                    offset += bye.Length;                                    
                                    break;
                                case 204: // app
                                    var app = new RtcpAppPacket();
                                    app.Parse(packets, offset);                                    
                                    OnPacketReceived(new RtcpPacketReceivedArgs(app));
                                    offset += app.Length;
                                    break;
                            }                            
                        }                        
                    }
                }
                finally
                {
                    switch (_transmissionMode)
                    {
                        case TransmissionMode.Multicast:
                            _udpClient.DropMulticastGroup(_multicastEndPoint.Address);
                            _udpClient.Close();
                            break;
                        case TransmissionMode.Unicast:
                            _udpClient.Close();
                            break;
                    }   
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SAT>IP : RTCP listener thread exception"), ex);
                return;
            }
            Logger.Warn("SAT>IP : RTCP listener thread stopping");
        }

        public delegate void PacketReceivedHandler(object sender, RtcpPacketReceivedArgs e);
        public event PacketReceivedHandler PacketReceived;
        private bool _disposed;
        
        protected void OnPacketReceived(RtcpPacketReceivedArgs args)
        {
            if (PacketReceived != null)
            {
                PacketReceived(this, args);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopRtcpListenerThread();
                }
            }
            _disposed = true;
        }
        public class RtcpPacketReceivedArgs : EventArgs
        {
            public Object Packet { get; private set; }

            public RtcpPacketReceivedArgs(Object packet)
            {
                Packet = packet;
            }
        }
    }
    
}
