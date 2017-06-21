using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Timers;
using System;

namespace Pxmart.Monitor.Item
{
    public class DeviceItem : ListViewItem, IDisposable
    {
        private string _ip;       /* 被監控的設備IP */
        private bool _status;   /* 被監控的設備狀態 */
        private int _maxFailed;   /* 測試連線失敗次數大於最大失敗次數後，才通知程式更新設備狀態 */
        private int _failedCount = 0; /* 連線失敗次數計算 */
        private bool _failedInfo = false;
        private bool disposed = false;
        private bool _first = false;

        private System.Timers.Timer _timer = new System.Timers.Timer();

        public bool Status
        {
            get
            {
                return _status;
            }
        }
        public bool Failed_Info
        {
            get
            {
                return _failedInfo;
            }
            set
            {
                _failedInfo = value;
            }
        }
        public int Failed_Count
        {
            get
            {
                return _failedCount;
            }
        }
        public int Failed_Max
        {
            get
            {
                return _maxFailed;
            }
        }
        public string IP
        {
            get
            {
                return _ip;
            }
        }

        public event DeviceFailedHandler OnDeviceFailed;
        public event DeviceSuccessHandler OnDeviceSuccess;
        public event DeviceStatusHandler OnDeviceStatus;

        public DeviceItem(string devIP, int maxFailed = 3, bool devStatus = false, int interval = 10000)
        {
            _ip = devIP;
            _maxFailed = maxFailed;
            _status = devStatus;
        
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Interval = interval;

            this.SubItems.Add("printer");
            this.SubItems.Add(_ip);
            this.SubItems.Add("");
            this.SubItems.Add("測試設備連線~~");

            if (_status)
            {
                this.StateImageIndex = 0;
                this.Text = "連線正常";
            }
            else
            {
                this.StateImageIndex = 1;
                this.Text = "連線中斷";
            }

            Start();
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Stop();
                _timer.Dispose();
            }

            disposed = true;
        }

        ~DeviceItem()
        {
            Dispose(false);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_first == false)
            {
                OnDeviceSuccess(this, new DeviceSuccessEventArgs(_ip));
                _first = true;
            }

            _status = PingIP(_ip);       
            OnDeviceStatus(this, new DeviceStatusEventArgs(this, _status, ""));

            if (_status == false)
            {
                _failedCount++;
                if (_failedCount > _maxFailed && _failedInfo == false)
                {
                    _failedInfo = true;                    
                    OnDeviceFailed(this, new DeviceFailedEventArgs(_ip));
                }
            }
            else if (_status == true && _failedInfo == true)            
            {
                _failedCount = 0;
                _failedInfo = false;
                OnDeviceSuccess(this, new DeviceSuccessEventArgs(_ip));
            }
        }

        public void Start()
        {            
            _timer.Start();            
        }

        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// 測試設備連線是否正常
        /// </summary>
        /// <param name="IPv4Address">IP Address</param>
        /// <returns>true/false</returns>
        public bool PingIP(string IPv4Address)
        {
            IPAddress ip = IPAddress.Parse(IPv4Address);
            Ping pingControl = new Ping();
            PingReply reply = pingControl.Send(ip);
            pingControl.Dispose();

            return (reply.Status != IPStatus.Success) ? false : true;
        }
    }
}
