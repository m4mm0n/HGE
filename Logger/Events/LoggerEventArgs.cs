using System;

namespace HGE.Logger.Events
{
    public class LoggerEventArgs : EventArgs
    {
        public LoggerEventArgs(string format, params object[] param)
        {
            Log = string.Format(format, param);
        }

        public string Log { get; set; }
    }
}