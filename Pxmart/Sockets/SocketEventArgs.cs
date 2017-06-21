using System;
using System.Net.Sockets;

namespace Pxmart.Sockets
{
    public class ConnectEventArgs : EventArgs
    {
        public ConnectEventArgs() : base()
        {
        }
    }
    public delegate void ConnectHandler(object sender, ConnectEventArgs e);

    public class DisconnectEventArgs : EventArgs
    {
        public DisconnectEventArgs() : base()
        {
        }
    }
    public delegate void DisconnectHandler(object sender, DisconnectEventArgs e);

    public class ReceiveEventArgs : EventArgs
    {
        string _message;
        public ReceiveEventArgs(string message) : base()
        {
            _message = message;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public delegate void ReceiveHandler(object sender, ReceiveEventArgs e);

    public class ErrorEventArgs : EventArgs
    {
        string _error;
        public ErrorEventArgs(string error) : base()
        {
            _error = error;
        }
        public string Error
        {
            get
            {
                return _error;
            }
        }
    }
    public delegate void ErrorHandler(object sender, ErrorEventArgs e);

    public class SendEventArgs : EventArgs
    {
        private string _message;
        public SendEventArgs(string message) : base()
        {
            _message = message;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public delegate void SendHandler(object sender, SendEventArgs e);

    public class ReturnLogEventArgs : EventArgs
    {
        private string _message;
        public ReturnLogEventArgs(string message) : base()
        {
            _message = message;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
    public delegate void ReturnLogEventHandler(object sender, ReturnLogEventArgs e);

    public class ListenEventArgs : EventArgs
    {
        public ListenEventArgs() : base()
        {
        }
    }
    public delegate void ListenHandler(object sender, ListenEventArgs e);

    public class AcceptEventArgs : EventArgs
    {
        public AcceptEventArgs() : base()
        { }
    }
    public delegate void AcceptHandler(object sender, AcceptEventArgs e);

    public class ClientReadEventArgs : EventArgs
    {
        private string _message;
        private Socket _socket;
        public ClientReadEventArgs(Socket socket, string message)
        {
            _message = message;
            _socket = socket;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
        public Socket Client
        {
            get
            {
                return _socket;
            }
        }
    }
    public delegate void ClientReadHandler(object sender, ClientReadEventArgs e);

    public class ClientWriteEventArgs : EventArgs
    {
        private string _message;
        private Socket _socket;
        public ClientWriteEventArgs(Socket socket, string message)
        {
            _message = message;
            _socket = socket;
        }
        public string Message
        {
            get
            {
                return _message;
            }
        }
        public Socket Client
        {
            get
            {
                return _socket;
            }
        }
    }
    public delegate void ClientWriteHandler(object sender, ClientWriteEventArgs e);

    public class ClientConnectEventArgs : EventArgs
    {
        private Socket _socket;
        public ClientConnectEventArgs(Socket socket)
        {
            _socket = socket;
        }
        public Socket Client
        {
            get
            {
                return _socket;
            }
        }
    }
    public delegate void ClientConnectHandler(object sender, ClientConnectEventArgs e);

    public class ClientDisconnectEventArgs : EventArgs
    {
        private Socket _socket;
        public ClientDisconnectEventArgs(Socket socket)
        {
            _socket = socket;
        }
        public Socket Client
        {
            get
            {
                return _socket;
            }
        }
    }
    public delegate void ClientDisconnectHandler(object sender, ClientDisconnectEventArgs e);
    
    public class ClientErrorEventArgs : EventArgs
    {
        private Socket _socket;
        private string _error;
        public ClientErrorEventArgs(Socket socket, string error)
        {
            _socket = socket;
            _error = error;
        }
        public Socket Client
        {
            get
            {
                return _socket;
            }
        }
        public string Error
        {
            get
            {
                return _error;
            }
        }
    }
    public delegate void ClientErrorHandler(object sender, ClientErrorEventArgs e);
    
}
