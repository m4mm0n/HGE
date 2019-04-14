using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HGE.Logger.Enumerations;
using HGE.Logger.Loggers;

namespace HGE.Logger
{
    public class Logger
    {
        private readonly LoggerType logType;
        private readonly bool useStackName;
        private ILogger conLogger;
        private ILogger filLogger;
        private string logOwner;

        public TraceLogger TraceListener = null;
        private bool useConsole;
        private bool useFile;

        public Logger(LoggerType lType, string lOwner = "", bool bUseStackName = false)
        {
            logType = lType;

            if ((logType & LoggerType.Console) == LoggerType.Console)
                conLogger = new ConsoleLogger("yyyy-MM-dd HH:mm:ss.fff", bUseStackName ? "" : lOwner);
            if ((logType & LoggerType.File) == LoggerType.File)
                filLogger = new FileLogger("yyyy-MM-dd HH:mm:ss.fff", lOwner);

            useStackName = bUseStackName;
            logType = lType;
        }

        ~Logger()
        {
            TraceListener?.Close();
            conLogger = null;
            filLogger = null;
        }

        public void Log(Exception ex)
        {
            var logString = ex.ToString();
            if (useStackName)
                logString = "[" + GetCurrentMethod() + "] " + logString;

            TraceListener?.WriteLine(logString);
            conLogger?.Exception(logString);
            filLogger?.Exception(logString);
        }

        public void Log(LogType lType, string format, params object[] args)
        {
            var logString = string.Format(format, args);
            if (useStackName)
                logString = "[" + GetCurrentMethod() + "] " + logString;

            TraceListener?.WriteLine(logString);
            switch (lType)
            {
                //case LogType.Trace:
                //    break;
#if DEBUG
                case LogType.Debug:
                    conLogger?.Debug(logString);
                    filLogger?.Debug(logString);
                    break;
#endif
                case LogType.Normal:
                    conLogger?.Normal(logString);
                    filLogger?.Normal(logString);
                    break;
                case LogType.Success:
                    conLogger?.Success(logString);
                    filLogger?.Success(logString);
                    break;
                case LogType.Failure:
                    conLogger?.Failure(logString);
                    filLogger?.Failure(logString);
                    break;
                case LogType.Warning:
                    conLogger?.Warning(logString);
                    filLogger?.Warning(logString);
                    break;
                case LogType.Error:
                    conLogger?.Error(logString);
                    filLogger?.Error(logString);
                    break;
                case LogType.Critical:
                    conLogger?.Critical(logString);
                    filLogger?.Critical(logString);
                    break;
                case LogType.Exception:
                    conLogger?.Exception(logString);
                    filLogger?.Exception(logString);
                    break;
                case LogType.Verbose:
                    conLogger?.Verbose(logString);
                    filLogger?.Verbose(logString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lType), lType, null);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }
    }
}