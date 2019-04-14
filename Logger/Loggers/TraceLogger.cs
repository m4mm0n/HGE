using System;
using System.Diagnostics;
using HGE.Logger.Events;

namespace HGE.Logger.Loggers
{
    public class TraceLogger : TraceListener
    {
        private bool hasBegunDispose = false;
        private bool hasStartedWriting;
        public event EventHandler<LoggerEventArgs> OnTraceEvent;

        protected virtual void AddLog(string format, params object[] args)
        {
            OnTraceEvent?.Invoke(this, new LoggerEventArgs(format, args));
        }

        public override void Write(string message)
        {
            if (!hasStartedWriting)
                AddLog("[{0}] {1}", DateTime.Now.ToShortTimeString(), message);
            else
                AddLog(" {0}", message);
        }

        public override void WriteLine(string message)
        {
            AddLog("[{0}] {1}{2}", DateTime.Now.ToShortTimeString(), message, Environment.NewLine);
            hasStartedWriting = false;
        }
    }
}