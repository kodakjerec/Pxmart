namespace Pxmart.Monitor.Command
{
    public class ServerCommand
    {
        public ServerCommand(string value) { Value = value; }
        public string Value { get; set; }

        /// <summary>
        /// 監控系統發送監控指令
        /// </summary>
        public static string Connection { get { return new ServerCommand("SE0000").Value; } }
        /// <summary>
        /// 詢問有幾台設備需要監控
        /// </summary>
        public static string MonitorCount { get { return new ServerCommand("SE1001").Value; } }
        /// <summary>
        /// 請求設備LIST
        /// </summary>
        public static string MonitorList { get { return new ServerCommand("SE1002").Value; } }
        /// <summary>
        /// 通知設備重試3次後傳送斷線
        /// </summary>
        public static string DeviceFail { get { return new ServerCommand("SE1100").Value; } }
        /// <summary>
        /// 通知設備斷線後又連線正常
        /// </summary>
        public static string DeviceSuccess { get { return new ServerCommand("SE1101").Value; } }
        /// <summary>
        /// 取得WARNING、ERROR等級的Log訊息
        /// </summary>
        public static string LogReturn { get { return new ServerCommand("SE8000").Value; } }        
        /// <summary>
        /// 終止監控
        /// </summary>
        public static string Disconnection { get { return new ServerCommand("SE9999").Value; } }
    }
}
