using System;

namespace HGE.Events
{
    public class LogEvent : EventArgs
    {
        public LogEvent(string format, params object[] param)
        {
            Log = "[" + DateTime.Now.ToShortTimeString() + "] " + string.Format(format, param);
        }

        public string Log { get; set; }
    }
}