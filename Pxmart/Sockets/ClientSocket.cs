using Microsoft.Win32.SafeHandles;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace Pxmart.Sockets
{
     public class ClientSocket : System.Net.Sockets.Socket
    {
        private static readonly int BUFFER_LENGTH = 8192;

        public event ConnectHandler OnConnect;
        public event DisconnectHandler OnDisconnect;       
        public event ReceiveHandler OnReceive;        
        public event ErrorHandler OnError;        
        public event SendHandler OnSend;

        protected virtual void RaiseConnectEvent()
        {
            if (OnConnect != null)
                OnConnect(this, new ConnectEventArgs());
        }

        protected virtual void RaiseDisconnectEvent()
        {
            if (OnDisconnect != null)
                OnDisconnect(this, new DisconnectEventArgs());
        }
        protected virtual void RaiseReceiveEvent(string message)
        {
            if (OnReceive != null)
                OnReceive(this, new ReceiveEventArgs(message));
        }
        protected virtual void RaiseErrorEvent(string error)
        {
            if (OnError != null)
                OnError(this, new ErrorEventArgs(error));
        }
        protected virtual void RaiseSendEvent(string message)
        {
            if (OnSend != null)
                OnSend(this, new SendEventArgs(message));
        }

        public ClientSocket() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {                    
        }

        public new bool Connect(EndPoint remoteEP)
        {
            try
            {
                base.Connect(remoteEP);
                RaiseConnectEvent();

                Thread recvTd = new Thread(DoReceive);
                recvTd.Start();
                return true;
            }
            catch (Exception ex)
            {                
                RaiseErrorEvent(ex.Message);
                return false;
            }
        }

        public new bool Connect(string host, int port)
        {
            try
            {
                base.Connect(host, port);

                RaiseConnectEvent();

                Thread recvTd = new Thread(DoReceive);
                recvTd.IsBackground = true;
                recvTd.Start();
                return true;
            }
            catch(Exception ex)
            {                
                RaiseErrorEvent(ex.Message);
                return false;
            }
        }

        public void DoReceive()
        {
            byte[] buffer = new byte[BUFFER_LENGTH];
            int recvCount = 0;
            string msg = string.Empty;
            bool closed = false;
            SocketError errorCode;

            //Send_Device_List();
            while (base.Connected && !closed)
            {
                if (base.Available > 0)
                {
                    recvCount = base.Receive(buffer, 0, buffer.Length, SocketFlags.None, out errorCode);
                    msg += Encoding.Default.GetString(buffer, 0, recvCount);
                }
                else
                {
                    if (msg != string.Empty)
                    {
                        RaiseReceiveEvent(msg);
                        msg = string.Empty;
                    }
                }
                try
                {
                    //使用Peek測試連線是否仍存在
                    if (base.Connected && base.Poll(0, SelectMode.SelectRead))
                        closed = base.Receive(buffer, SocketFlags.Peek) == 0;
                }
                catch (SocketException se)
                {
                    closed = true;
                    RaiseErrorEvent(se.Message);
                }

                if (closed)
                {
                    RaiseDisconnectEvent();
                }
                Thread.Sleep(1);
            }
        }

        public void Send(string msg)
        {
            try
            {
                base.Send(Encoding.Default.GetBytes(msg));
                RaiseSendEvent(msg);
            }
            catch(Exception ex)
            {
                RaiseErrorEvent(ex.Message);
            }
        }
    }
}
