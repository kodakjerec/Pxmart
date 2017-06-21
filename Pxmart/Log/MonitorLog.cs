using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Pxmart.Log
{
    public enum LogLevel : int
    {
        DEBUG = 0,
        INFO = 1,
        WARNING = 2,
        ERROR = 3,
        REMOTE = 4
    }

    public class MonitorLog
    {
        private System.Object lockThis = new System.Object();

        public string FilePath { get; set; }
        public string AppName { get; set; }
        public LogLevel Level { get; set; }
     
        public void Write(string format, params object[] arg)
        {
            Write(string.Format(format, arg));
        }
     
        public void Write(LogLevel level, string message)
        {
            if (level < Level) return;

            if (string.IsNullOrEmpty(FilePath))
            {
                //FilePath = Directory.GetCurrentDirectory();
                FilePath = Application.StartupPath;
            }

            if (AppName == string.Empty)
            {
                AppName = "None";
            }

            string filename = FilePath +
                    string.Format("\\Log\\{0}_{1:yyyy-MM-dd}.txt", AppName, DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }
            string writeString = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1,-8} {2}",
                    DateTime.Now, level , message) +Environment.NewLine;
            lock(lockThis)
            {
                File.AppendAllText(filename, writeString, Encoding.Unicode);
            }
        }
    }
}
