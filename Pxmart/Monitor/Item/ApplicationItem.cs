using System.Collections.Generic;
using System.Net.Sockets;
using Pxmart.Sockets;
using Pxmart.Monitor.Command;
using Pxmart.Log;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System;
using System.IO;

/*
 * 2017.06.21 Kota  1. 程式啟動功能獨立成函數_processStart
 *                  2. 修改socket重新連線機制
 */

namespace Pxmart.Monitor.Item
{
    public class ApplicationItem : ListViewItem
    {
        public bool myStatus = true;
        private ClientSocket _socket;
        private string _name;
        private string _ip;
        private int _port;
        private string _path;
        private int _monitorCount;
        private string _logMessage = string.Empty;
        private List<DeviceItem> Device_List = new List<DeviceItem>();
        private Process _process = new Process();
        private MonitorLog _log = new MonitorLog();
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private static readonly int _interval = 10000;
        private System.Object lockThis = new System.Object();

        public List<string> Log_List = new List<string>();

        #region event function
        public event AppStatusHandler OnAppStatusChange;
        public event AppAddDeviceItemHandler OnAddDeviceItem;
        public event AppRemoveDeviceItemHandler OnRemoveDeviceItem;
        public event DeviceStatusHandler OnDeviceStatusChange;
        public event ReturnLogEventHandler OnReturnLog;
        protected virtual void RaiseAppStatusChangeEvent(ApplicationItem item, bool status, string message)
        {
            if (OnAppStatusChange != null)
                OnAppStatusChange(this, new AppStatusEventArgs(item, status, message));
        }
        protected virtual void RaiseAddDeviceItemEvent(DeviceItem item)
        {
            if (OnAddDeviceItem != null)
                OnAddDeviceItem(this, new AppAddDeviceItemEventArgs(item));
        }
        protected virtual void RaiseRemoveDeviceItemEvent(DeviceItem item)
        {
            if (OnRemoveDeviceItem != null)
                OnRemoveDeviceItem(this, new AppRemoveDeviceItemEventArgs(item));
        }
        protected virtual void RaiseDeviceStatusChangeEvent(DeviceItem item, bool status, string message)
        {
            if (OnDeviceStatusChange != null)
                OnDeviceStatusChange(this, new DeviceStatusEventArgs(item, status, message));
        }
        protected virtual void RaiseReturnLog(string message)
        {
            if (OnReturnLog != null)
                OnReturnLog(this, new ReturnLogEventArgs(message));
        }
        #endregion

        /// <summary>
        /// 重新開啟程式
        /// </summary>
        private bool _processStart(string path, string arguments)
        {
            if (_process.StartInfo.FileName == string.Empty)
            {
                _process.StartInfo.FileName = _path;
                _process.StartInfo.Arguments = arguments;
                _process.StartInfo.WorkingDirectory = Path.GetDirectoryName(_path);
            }

            try
            {
                #region 判斷程式是否已經在執行
                string filename = Path.GetFileName(_process.StartInfo.FileName).Replace(".exe", "");

                // 取得欲控制程式的名稱
                Process[] p = Process.GetProcessesByName(filename);

                // 判斷是否為長度大於 0
                if (p.Length > 0)
                {
                    //已經在執行, 開啟視窗
                    //int hwnd = p[0].MainWindowHandle.ToInt32();
                    //clsShowWindow.ShowWindow(hwnd, (int)clsShowWindow.CommandShow.SW_SHOWDEFAULT);
                    //return;
                }
                else
                {
                    _process.Start();
                }
                #endregion
                Start();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ApplicationItem(string name, string ip, string port, string path, string arguments)
        {
            _name = name;
            _ip = ip;
            try
            {
                _port = int.Parse(port);
            }
            catch
            {
                _port = 0;
            }
            _path = path;

            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = _interval;

            //開啟程式
            bool IsOk = _processStart(path, arguments);
            if (IsOk)
            {
                _logMessage = "程式已開啟";
                this.StateImageIndex = 0;
                this.Text = _logMessage;
                _log.Write(LogLevel.INFO, _logMessage);
            }
            else
            {
                _logMessage = "程式無法開啟";
                this.StateImageIndex = 0;
                this.Text = _logMessage;
                _log.Write(LogLevel.INFO, _logMessage);
            }

            _log.AppName = _name;
            _log.Level = LogLevel.INFO;

            _socket = new ClientSocket();
            _socket.OnConnect += new ConnectHandler(OnConnect);
            _socket.OnDisconnect += new DisconnectHandler(OnDisconnect);
            _socket.OnReceive += new ReceiveHandler(OnReceive);
            _socket.OnError += new ErrorHandler(OnError);
            _socket.OnSend += new SendHandler(OnSend);

            this.SubItems.Add(_name);
            this.SubItems.Add(_ip);
            this.SubItems.Add(_port.ToString());
            this.SubItems.Add(_logMessage);
            this.SubItems.Add(arguments);
            this.SubItems.Add(_path);

            if (_ip != string.Empty)
            {
                _socket.Connect(_ip, _port);
                _logMessage = "建立連線~~";
                _log.Write(LogLevel.INFO, _logMessage);

                if (_socket.Connected)
                {
                    _logMessage = "連線正常";
                    this.StateImageIndex = 0;
                    this.Text = _logMessage;
                    _log.Write(LogLevel.INFO, _logMessage);
                }
                else
                {
                    _logMessage = "連線中斷";
                    this.StateImageIndex = 1;
                    this.Text = _logMessage;
                    _log.Write(LogLevel.ERROR, _logMessage);
                    Start();
                }
            }
        }

        /// <summary>
        /// timer Start
        /// </summary>
        public void Start()
        {
            if (myStatus == true)
                _timer.Start();
        }

        /// <summary>
        /// timer Stop
        /// </summary>
        public void Stop()
        {
            if (myStatus == true)
                _timer.Stop();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CreatNewSocket();
            if (_socket.Connected == true)
            {
                OnEventLogProcess(LogLevel.WARNING, "重新建立連線成功~~");
                Stop();
            }
        }

        private void OnEventLogProcess(LogLevel level, string message)
        {
            lock (lockThis)
            {
                _log.Write(level, message);
                RaiseAppStatusChangeEvent(this, _socket.Connected, message);
                RaiseReturnLog(_name + "-" + _ip + ":" + _port + " " + message);
            }
        }

        /// <summary>
        /// 定時檢查連線狀態
        /// </summary>
        private void CreatNewSocket()
        {
            //2017.06.21 Kota 修改程式啟動方式
            if (_path != string.Empty)
            {
                bool IsOk = _processStart("", "");
                if (IsOk)
                {
                    _logMessage = "程式已開啟";
                    this.StateImageIndex = 0;
                    this.Text = _logMessage;
                    _log.Write(LogLevel.INFO, _logMessage);
                }
                else
                {
                    _logMessage = "程式無法開啟";
                    this.StateImageIndex = 0;
                    this.Text = _logMessage;
                    _log.Write(LogLevel.INFO, _logMessage);
                }
            }

            //2017.06.21 Kota 修改socket重新連線機制
            if (_ip != string.Empty)
            {
                if (_socket.Connected == false)
                {
                    _socket = new ClientSocket();
                    _socket.OnConnect += new ConnectHandler(OnConnect);
                    _socket.OnDisconnect += new DisconnectHandler(OnDisconnect);
                    _socket.OnReceive += new ReceiveHandler(OnReceive);
                    _socket.OnError += new ErrorHandler(OnError);
                    _socket.OnSend += new SendHandler(OnSend);
                    _socket.Connect(_ip, _port);
                }
            }
        }

        ~ApplicationItem()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        private void OnConnect(object sender, ConnectEventArgs e)
        {
            Connection();
        }

        private void OnSend(object sender, SendEventArgs e)
        {
            _log.Write(LogLevel.INFO, e.Message);
        }

        private void OnError(object sender, Sockets.ErrorEventArgs e)
        {
            OnEventLogProcess(LogLevel.ERROR, e.Error);
            Start();
        }

        private void OnReceive(object sender, ReceiveEventArgs e)
        {
            _log.Write(LogLevel.DEBUG, e.Message);
            string[] commands = e.Message.Split('#');

            foreach (string command in commands)
            {
                string[] para = command.Split('^');

                string cmd = new ClientCommand(para[0]).Value;

                if (cmd == ClientCommand.Connection)
                {
                    MonitorCount();
                    continue;
                }
                if (cmd == ClientCommand.MonitorCount)
                {
                    _monitorCount = int.Parse(para[1]);
                    MonitorList();
                    continue;
                }
                if (cmd == ClientCommand.MonitorList)
                {
                    string[] devList = para[1].Split(';');
                    foreach (string ip in devList)
                    {
                        if (ip != string.Empty)
                        {
                            if (!Device_List.Exists(t => t.IP == ip))
                            {
                                DeviceItem item = new DeviceItem(ip);
                                item.OnDeviceFailed += new DeviceFailedHandler(OnDeviceFailed);
                                item.OnDeviceSuccess += new DeviceSuccessHandler(OnDeviceSuccess);
                                item.OnDeviceStatus += new DeviceStatusHandler(OnDeviceStatus);

                                Device_List.Add(item);
                                RaiseAddDeviceItemEvent(item);
                            }
                        }
                    }
                    continue;
                }
                if (cmd == ClientCommand.DeviceFail)
                {
                    continue;
                }
                if (cmd == ClientCommand.DeviceSuccess)
                {
                    continue;
                }
                if (cmd == ClientCommand.LogReturn)
                {
                    _log.Write(LogLevel.REMOTE, para[1]);
                    continue;
                }
                if (cmd == ClientCommand.Disconnection)
                {
                    continue;
                }
            }
        }

        private void OnDeviceStatus(object sender, DeviceStatusEventArgs e)
        {
            RaiseDeviceStatusChangeEvent(e.Item, e.Status, e.Message);
        }

        private void OnDeviceSuccess(object sender, DeviceSuccessEventArgs e)
        {
            DeviceSuccess(e.IP);
            OnEventLogProcess(LogLevel.WARNING, e.IP + "連線成功。");
        }

        private void OnDeviceFailed(object sender, DeviceFailedEventArgs e)
        {
            DeviceFail(e.IP);
            OnEventLogProcess(LogLevel.WARNING, e.IP + "斷線。");
        }

        private void OnDisconnect(object sender, DisconnectEventArgs e)
        {
            try
            {
                OnEventLogProcess(LogLevel.ERROR, "連線不正常終止");

                foreach (DeviceItem item in Device_List)
                    RaiseRemoveDeviceItemEvent(item);

                Device_List.Clear();

                //if (_path != string.Empty)
                //{
                //    _process.Start();
                //}
                OnEventLogProcess(LogLevel.ERROR, "無法建立連線，10秒重新建立連線~~");
                Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #region Socket Command Function
        public void Connection()
        {
            _socket.Send(ServerCommand.Connection + "#");
        }

        public void MonitorCount()
        {
            _socket.Send(ServerCommand.MonitorCount + "#");
        }

        public void MonitorList()
        {
            _socket.Send(ServerCommand.MonitorList + "#");
        }

        public void DeviceFail(string ip)
        {
            _socket.Send(ServerCommand.DeviceFail + "^" + ip + "#");
            Thread.Sleep(1000);
        }

        public void DeviceSuccess(string ip)
        {
            _socket.Send(ServerCommand.DeviceSuccess + "^" + ip + "#");
            Thread.Sleep(1000);
        }

        public void LogReturn(string log)
        {
            _socket.Send(ServerCommand.LogReturn + "^" + log + "#");
        }

        public void Disconnection()
        {
            _socket.Send(ServerCommand.Disconnection + "#");
        }
        #endregion
    }
}
