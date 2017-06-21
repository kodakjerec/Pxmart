using System;
using System.Windows.Forms;
using Pxmart.Monitor.Item;
using Pxmart.Utility;
using Pxmart.Log;
using Pxmart.Sockets;
using Pxmart.Monitor.Command;
using System.Net;
using System.Net.Sockets;

#region Modify log
/*
 * 2017.06.21 Kota  1. 修改本機IP取得方式
 *                  2. 介面增加ApplicationItem停用功能
 */
#endregion

namespace PxmartMonitor
{
    public partial class FormMain : Form
    {
        private MonitorLog _log = new MonitorLog();
        private ServerSocket server = new ServerSocket();

        #region GUI Delegate Declarations
        public delegate void Add_Device_Item(DeviceItem item);
        private Add_Device_Item addMethod;
        public delegate void Remove_Device_Item(DeviceItem item);
        private Remove_Device_Item removeMethod;
        public delegate void Item_Status_Update(ListViewItem item, bool status, string message);
        public Item_Status_Update itemStatus;
        #endregion

        #region Delegate Functions 
        public void DoStatusUpdate(ListViewItem item, bool status, string message)
        {
            try
            {
                if (status)
                {
                    view_DevicesList.Items[item.Index].StateImageIndex = 0;
                    view_DevicesList.Items[item.Index].Text = "連線正常";
                }
                else
                {
                    view_DevicesList.Items[item.Index].StateImageIndex = 1;
                    view_DevicesList.Items[item.Index].Text = "連線中斷";
                }

                view_DevicesList.Items[item.Index].SubItems[4].Text = message;
            }
            catch
            { }
        }
        public void DoAddDeviceItem(DeviceItem item)
        {
            view_DevicesList.Items.Add(item);
        }
        public void DoRemoveDeviceItem(DeviceItem item)
        {
            view_DevicesList.Items.Remove(item);
            item.Dispose();
        }

        #endregion

        public FormMain()
        {
            InitializeComponent();
            itemStatus = new Item_Status_Update(DoStatusUpdate);

            server.OnListen += new ListenHandler(Server_OnListen);
            server.OnAccept += new AcceptHandler(Server_OnAccept);
            server.OnError += new ErrorHandler(Server_OnError);
            server.OnClientConnect += new ClientConnectHandler(Server_OnClientConnect);
            server.OnClientDisconnect += new ClientDisconnectHandler(Server_OnClientDisconnect);
            server.OnClientRead += new ClientReadHandler(Server_OnClientRead);
            server.OnClientWrite += new ClientWriteHandler(Server_OnClientWrite);
            server.OnClientError += new ClientErrorHandler(Server_OnClientError);

            server.Bind(4949);
            server.Listen(100);
            server.Accept();
        }

        #region Monitor Server Event Function
        private void Server_OnClientError(object sender, ClientErrorEventArgs e)
        {

        }

        private void Server_OnClientWrite(object sender, ClientWriteEventArgs e)
        {

        }

        private void Server_OnClientRead(object sender, ClientReadEventArgs e)
        {
            string[] commands = e.Message.Split('#');

            foreach (string command in commands)
            {
                string[] para = command.Split('^');

                string serverCmd = new ServerCommand(para[0]).Value;

                if (serverCmd == ServerCommand.Connection)
                {
                    server.Send(e.Client, ClientCommand.Connection + "#");
                }

                if (serverCmd == ServerCommand.MonitorCount)
                {
                    server.Send(e.Client, ClientCommand.MonitorCount + "^0#");
                }

                if (serverCmd == ServerCommand.MonitorList)
                {
                    server.Send(e.Client, ClientCommand.MonitorList + "^;#");
                }

                if (serverCmd == ServerCommand.DeviceFail)
                {
                    server.Send(e.Client, ClientCommand.DeviceFail + "#");
                }

                if (serverCmd == ServerCommand.DeviceSuccess)
                {
                    server.Send(e.Client, ClientCommand.DeviceSuccess + "#");
                }

                if (serverCmd == ServerCommand.Disconnection)
                {
                    server.Send(e.Client, ClientCommand.Disconnection + "#");
                }
            }
        }

        private void Server_OnError(object sender, ErrorEventArgs e)
        {

        }

        private void Server_OnClientDisconnect(object sender, ClientDisconnectEventArgs e)
        {

        }

        private void Server_OnClientConnect(object sender, ClientConnectEventArgs e)
        {

        }

        private void Server_OnAccept(object sender, AcceptEventArgs e)
        {

        }

        private void Server_OnListen(object sender, ListenEventArgs e)
        {

        }
        #endregion

        private void FormMain_Load(object sender, EventArgs e)
        {
            //2017.06.21 Kota  1. 修改本機IP取得方式
            string localIP = "";
            //取得本機IP
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            this.Text = "全聯-設備監控系統: " + localIP;// Dns.Resolve(Dns.GetHostName()).AddressList[0].ToString();

            addMethod = new Add_Device_Item(this.DoAddDeviceItem);
            removeMethod = new Remove_Device_Item(this.DoRemoveDeviceItem);

            XMLTool xml = new XMLTool(Application.StartupPath + "/Setup.xml");

            string[] appList = xml.GetChildAttributeText("/setup/monitor/application", '^', "name", "ip", "port", "path", "arguments");
            foreach (string app in appList)
            {
                string[] appInfo = app.Split('^');

                //Thread.Sleep(1000);
                ApplicationItem appItem = new ApplicationItem(appInfo[0], appInfo[1], appInfo[2], appInfo[3], appInfo[4]);
                appItem.OnAppStatusChange += new AppStatusHandler(OnAppStatusChange);
                appItem.OnAddDeviceItem += new AppAddDeviceItemHandler(OnAddDeviceItem);
                appItem.OnRemoveDeviceItem += new AppRemoveDeviceItemHandler(OnRemoveDeviceItem);
                appItem.OnDeviceStatusChange += new DeviceStatusHandler(OnDeviceStatusChange);
                appItem.OnReturnLog += new ReturnLogEventHandler(OnReturnLog);
                view_DevicesList.Items.Add(appItem);
            }

            view_DevicesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void OnReturnLog(object sender, ReturnLogEventArgs e)
        {
            server.Broadcast(ClientCommand.LogReturn + "^" + e.Message + "#");
        }

        private void OnRemoveDeviceItem(object sender, AppRemoveDeviceItemEventArgs e)
        {
            this.BeginInvoke(removeMethod, e.Item);
        }

        private void OnDeviceStatusChange(object sender, DeviceStatusEventArgs e)
        {
            this.Invoke(this.itemStatus, e.Item, e.Status, e.Message);
        }

        private void OnAddDeviceItem(object sender, AppAddDeviceItemEventArgs e)
        {
            this.BeginInvoke(addMethod, e.Item);
        }

        private void OnAppStatusChange(object sender, AppStatusEventArgs e)
        {
            this.Invoke(this.itemStatus, e.Item, e.Status, e.Message);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// ApplicationItem停用功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void view_DevicesList_ItemActivate(object sender, EventArgs e)
        {
            //2017.06.21 介面增加ApplicationItem停用功能
            int i = view_DevicesList.SelectedIndices[0];
            try
            {
                ApplicationItem obj = (ApplicationItem)view_DevicesList.Items[i];


                if (obj.myStatus)
                {
                    obj.BackColor = System.Drawing.Color.LightGray;
                    obj.Stop();
                    obj.Text = "PAUSE";
                    obj.myStatus = false;
                }
                else
                {
                    obj.myStatus = true;
                    obj.Text = "START";
                    obj.Start();
                    obj.BackColor = System.Drawing.Color.White;
                }
            }
            catch
            {
                //不是ApplicationItem, 不做事
            }
        }
    }
}
