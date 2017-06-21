using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Pxmart.Sockets
{
    public class ServerSocket : Socket
    {
        private static readonly int BUFFER_LENGTH = 8192;
        private Socket[] sockets;
        private int _sockIndex = 0;

        public event ListenHandler OnListen;
        public event AcceptHandler OnAccept;
        public event ErrorHandler OnError;
        public event ClientConnectHandler OnClientConnect;
        public event ClientDisconnectHandler OnClientDisconnect;
        public event ClientReadHandler OnClientRead;
        public event ClientWriteHandler OnClientWrite;
        public event ClientErrorHandler OnClientError;

        protected virtual void RaiseListeEvent()
        {
            if (OnListen != null)
                OnListen(this, new ListenEventArgs());
        }

        protected virtual void RaiseAcceptEvent()
        {
            if (OnAccept != null)
                OnAccept(this, new AcceptEventArgs());
        }

        protected virtual void RaiseErrorEvent(string error)
        {
            if (OnError != null)
                OnError(this, new ErrorEventArgs(error));
        }

        protected virtual void RaiseClientConnectEvent(Socket socket)
        {
            if (OnClientConnect != null)
                OnClientConnect(this, new ClientConnectEventArgs(socket));
        }

        protected virtual void RaiseClientDisconnectEvent(Socket socket)
        {
            try
            {
                if (OnClientDisconnect != null)
                {
                    if (socket != null)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    OnClientDisconnect(this, new ClientDisconnectEventArgs(socket));
                }
            }
            catch
            { }
        }

        protected virtual void RaiseClientReadEvent(Socket socket, string message)
        {
            if (OnClientRead != null)
                OnClientRead(this, new ClientReadEventArgs(socket, message));
        }

        protected virtual void RaiseClientWriteEvent(Socket socket, string message)
        {
            if (OnClientWrite != null)
                OnClientWrite(this, new ClientWriteEventArgs(socket, message));
        }

        protected virtual void RaiseClientErrorEvent(Socket socket,string error)
        {
            if (OnClientError != null)
                OnClientError(this, new ClientErrorEventArgs(socket, error));
        }

        public ServerSocket() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
        }

        public void Bind(int port)
        {
            try
            {
                base.Bind(new IPEndPoint(IPAddress.Any, port));
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(ex.Message);
            }
        }

        public new void Listen(int backlog)
        {
            try
            {
                Array.Resize(ref sockets, 1);
                base.Listen(backlog);
                RaiseListeEvent();                
            }
            catch (Exception ex)
            {
                RaiseErrorEvent(ex.Message);
            }
        }

        public new void Accept()
        {            
            Thread acceptTd = new Thread(Socket_Wait_Accept);
            acceptTd.IsBackground = true;
            acceptTd.Start();
        }

        private void Socket_Wait_Accept()
        {
            bool entpyFlag = false;

            for (int i = 0; i <= sockets.Length - 1; i++)
            {
                // Sock[i] 若不為 null 表示已被實作過, 判斷是否有 Client 端連線
                if (sockets[i] != null)
                {
                    // 如果目前第 i 個 Socket 若沒有人連線, 便可提供給下一個 Client 進行連線
                    if (sockets[i].Connected == false)
                    {
                        _sockIndex = i;
                        entpyFlag = true;
                        break;
                    }
                }
            }

            // 沒有多餘的Socket可以連線，加大一個供Client連線
            if (entpyFlag == false)
            {
                _sockIndex = sockets.Length;
                Array.Resize(ref sockets, _sockIndex + 1);
            }

            Socket_Accept_Process();
        }

        private void Socket_Accept_Process()
        {
            int index = _sockIndex;

            try
            {              
                sockets[index] = base.Accept();

                RaiseAcceptEvent();

                RaiseClientConnectEvent(sockets[index]);

                Thread recvTd = new Thread(DoReceive);
                recvTd.IsBackground = true;
                recvTd.Start(sockets[index]);

                Socket_Wait_Accept();   // 再產生另一個執行緒等待下一個 Client 連線  
            }
            catch (Exception ex)
            {
                sockets[index].Shutdown(SocketShutdown.Both);
                sockets[index].Close();

                RaiseErrorEvent(ex.Message);

                Socket_Wait_Accept();
            }
        }

        private void DoReceive(object handle)
        {
            Socket client = (Socket)handle;
            byte[] buffer = new byte[BUFFER_LENGTH];
            int recvCount = 0;
            string msg = string.Empty;
            bool closed = false;
            SocketError errorCode;

            //Send_Device_List();
            while (client.Connected && !closed)
            {
                if (client.Available > 0)
                {
                    recvCount = client.Receive(buffer, 0, buffer.Length, SocketFlags.None, out errorCode);
                    msg += Encoding.Default.GetString(buffer, 0, recvCount);
                }
                else
                {
                    if (msg != string.Empty)
                    {
                        RaiseClientReadEvent(client, msg);
                        msg = string.Empty;
                    }
                }
                try
                {
                    //使用Peek測試連線是否仍存在
                    if (client.Connected && client.Poll(0, SelectMode.SelectRead))
                        closed = client.Receive(buffer, SocketFlags.Peek) == 0;
                }
                catch (SocketException se)
                {
                    closed = true;
                    RaiseClientErrorEvent(client, se.Message);
                }

                if (closed == true && client.Connected == false)
                {
                    RaiseClientDisconnectEvent(client);
                }
                Thread.Sleep(1);
            }
        }

        public void Send(Socket client, string msg)
        {
            try
            {
                client.Send(Encoding.Default.GetBytes(msg));
                RaiseClientWriteEvent(client, msg);
            }
            catch
            {
                RaiseClientDisconnectEvent(client);
            }
        }

        public void Broadcast(string msg)
        {
            foreach(Socket client in sockets)
            {
                Send(client, msg);
            }
        }
    }
}
