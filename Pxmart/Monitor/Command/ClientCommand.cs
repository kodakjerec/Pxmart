namespace Pxmart.Monitor.Command
{
    public class ClientCommand
    {
        public ClientCommand(string value) { Value = value; }
        public string Value { get; set; }

        /// <summary>
        /// 回覆接受監控並回傳程式的網路檔案位置
        /// </summary>
        public static string Connection { get { return new ClientCommand("CL0000").Value; } }
        /// <summary>
        /// 回覆總幾幾台設備需要監控
        /// </summary>
        public static string MonitorCount { get { return new ClientCommand("CL1001").Value; } }
        /// <summary>
        /// 回覆設備LIST
        /// </summary>
        public static string MonitorList { get { return new ClientCommand("CL1002").Value; } }
        /// <summary>
        /// 收到設備斷線
        /// </summary>
        public static string DeviceFail { get { return new ClientCommand("CL1100").Value; } }
        /// <summary>
        /// 收到設備連線正常
        /// </summary>
        public static string DeviceSuccess { get { return new ClientCommand("CL1101").Value; } }
        /// <summary>
        /// 回覆WARNING、ERROR等級的Log訊息
        /// </summary>
        public static string LogReturn { get { return new ClientCommand("CL8000").Value; } }
        /// <summary>
        /// 結束連線
        /// </summary>
        public static string Disconnection { get { return new ClientCommand("CL9999").Value; } }
    }
}
