using System;

namespace Pxmart.Monitor.Item
{
    public class DeviceFailedEventArgs : EventArgs
        {
        private string _ip;
        public DeviceFailedEventArgs(string ip) : base()
            {
                _ip = ip;
            }
        public string IP
            {
                get
                {
                    return _ip;
                }
            }
            
    }
    public delegate void DeviceFailedHandler(object sender, DeviceFailedEventArgs e);

    public class DeviceSuccessEventArgs : EventArgs
    {
        private string _ip;
        public DeviceSuccessEventArgs(string ip) : base()
            {
                _ip = ip;
            }
        public string IP
            {
                get
                {
                    return _ip;
                }
            }
    }
    public delegate void DeviceSuccessHandler(object sender, DeviceSuccessEventArgs e);

    public class DeviceStatusEventArgs : EventArgs
    {
        private DeviceItem _device;
        private bool _status;
        private string _message;
        public DeviceStatusEventArgs(DeviceItem device, bool status, string message) : base()
        {
            _device = device;
            _status = status;
            _message = message;
        }
        public bool Status
        {
            get
            {
                return _status;
            }
        }
        public DeviceItem Item
        {
            get
            {
                return _device;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public delegate void DeviceStatusHandler(object sender, DeviceStatusEventArgs e);

    public class AppStatusEventArgs : EventArgs
    {
        private ApplicationItem _app;
        private bool _status;
        private string _message;
        public AppStatusEventArgs(ApplicationItem app, bool status, string message) : base()
        {
            _app = app;
            _status = status;
            _message = message;
        }
        public bool Status
        {
            get
            {
                return _status;
            }
        }
        public ApplicationItem Item
        {
            get
            {
                return _app;
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public delegate void AppStatusHandler(object sender, AppStatusEventArgs e);

    public class AppAddDeviceItemEventArgs : EventArgs
    {
        private DeviceItem _item;
        public AppAddDeviceItemEventArgs(DeviceItem item) : base()
        {
            _item = item;
        }
        public DeviceItem Item
        {
            get
            {
                return _item;
            }
        }
        
    }
    public delegate void AppAddDeviceItemHandler(object sender, AppAddDeviceItemEventArgs e);

    public class AppRemoveDeviceItemEventArgs : EventArgs
    {
        private DeviceItem _item;
        public AppRemoveDeviceItemEventArgs(DeviceItem item) : base ()
        {
            _item = item;
        }
        public DeviceItem Item
        {
            get
            {
                return _item;
            }        
        }
    }
    public delegate void AppRemoveDeviceItemHandler(object sender, AppRemoveDeviceItemEventArgs e);
}
