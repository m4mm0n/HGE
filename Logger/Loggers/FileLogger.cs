using System;
using System.IO;
using System.Text;
using HGE.Logger.Enumerations;

namespace HGE.Logger.Loggers
{
    public class FileLogger : ILogger
    {
        private readonly string datetimeFormat;
        private readonly string logFilename;
        private readonly string logOwner;

        public FileLogger(string DateTimeFormat, string LogOwner)
        {
            datetimeFormat = DateTimeFormat;
            logOwner = LogOwner;

            if (!Directory.Exists("\\LOGS\\"))
                Directory.CreateDirectory("LOGS");

            logFilename = "LOGS\\" + logOwner + ".log";

            var logHeader = string.Format("[{0}]{1}", logOwner, Environment.NewLine);
            if (!File.Exists(logFilename))
                WriteLine(logHeader, false);
            else
                WriteLine(Environment.NewLine + Environment.NewLine + "[START NEW LOGGING PROCESS: " +
                          DateTime.Now.ToString(datetimeFormat) + "]" + Environment.NewLine);
        }

        public void Critical(string format, params object[] args)
        {
            WriteFile(LogType.Critical, string.Format(format, args));
        }
#if DEBUG
        public void Debug(string format, params object[] args)
        {
            WriteFile(LogType.Debug, string.Format(format, args));
        }
#endif
        public void Error(string format, params object[] args)
        {
            WriteFile(LogType.Error, string.Format(format, args));
        }

        public void Exception(string format, params object[] args)
        {
            WriteFile(LogType.Exception, string.Format(format, args));
        }

        public void Exception(Exception ex)
        {
            WriteFile(LogType.Exception, ex.ToString());
        }

        public void Failure(string format, params object[] args)
        {
            WriteFile(LogType.Failure, string.Format(format, args));
        }

        public void Normal(string format, params object[] args)
        {
            WriteFile(LogType.Normal, string.Format(format, args));
        }

        public void Success(string format, params object[] args)
        {
            WriteFile(LogType.Success, string.Format(format, args));
        }

        public void Verbose(string format, params object[] args)
        {
            WriteFile(LogType.Verbose, string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            WriteFile(LogType.Warning, string.Format(format, args));
        }

        private void WriteFile(LogType level, string text)
        {
            string pretext;

            switch (level)
            {
                case LogType.Normal:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [INFO]    ";
                    break;
                case LogType.Debug:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                    break;
                case LogType.Warning:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [WARNING] ";
                    break;
                case LogType.Error:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                    break;
                case LogType.Critical:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [CRITICAL]   ";
                    break;
                case LogType.Exception:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [EXCEPTION]   ";
                    break;
                case LogType.Failure:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [FAILURE]   ";
                    break;
                case LogType.Success:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [SUCCESS]   ";
                    break;
                case LogType.Verbose:
                    pretext = DateTime.Now.ToString(datetimeFormat) + " [VERBOSE]   ";
                    break;
                default:
                    pretext = "";
                    break;
            }

            WriteLine(pretext + text);
        }

        private void WriteLine(string text, bool append = true)
        {
            try
            {
                using (var writer = new StreamWriter(logFilename, append, Encoding.UTF8))
                {
                    if (text != "") writer.WriteLine(text);
                }
            }
            catch
            {
                //throw;
            }
        }
    }
}