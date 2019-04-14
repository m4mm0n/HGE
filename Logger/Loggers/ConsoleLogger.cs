using System;
using HGE.Logger.Enumerations;

namespace HGE.Logger.Loggers
{
    public class ConsoleLogger : ILogger
    {
        private readonly string datetimeFormat;
        private readonly string logOwner;

        public ConsoleLogger(string dateTimeFormat, string LogOwner)
        {
            datetimeFormat = dateTimeFormat;
            logOwner = LogOwner;
        }

        public void Success(string format, params object[] args)
        {
            WriteConsole(LogType.Success, string.Format(format, args));
        }

        public void Failure(string format, params object[] args)
        {
            WriteConsole(LogType.Failure, string.Format(format, args));
        }

        public void Warning(string format, params object[] args)
        {
            WriteConsole(LogType.Warning, string.Format(format, args));
        }

        public void Error(string format, params object[] args)
        {
            WriteConsole(LogType.Error, string.Format(format, args));
        }

        public void Exception(string format, params object[] args)
        {
            WriteConsole(LogType.Exception, string.Format(format, args));
        }

        public void Exception(Exception ex)
        {
            WriteConsole(LogType.Exception, string.Format("Exception: {0}", ex));
        }

        public void Critical(string format, params object[] args)
        {
            WriteConsole(LogType.Critical, string.Format(format, args));
        }

        public void Normal(string format, params object[] args)
        {
            WriteConsole(LogType.Normal, string.Format(format, args));
        }

        public void Verbose(string format, params object[] args)
        {
            WriteConsole(LogType.Verbose, string.Format(format, args));
        }

#if DEBUG
        public void Debug(string format, params object[] args)
        {
            WriteConsole(LogType.Debug, string.Format(format, args));
        }
#endif
        private void WriteConsole(LogType level, string text)
        {
            var orgCol = Console.ForegroundColor;

            switch (level)
            {
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("SUCCESS");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Normal:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" -> ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARNING");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Critical:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("CRITICAL");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("VERBOSE");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("DEBUG");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Exception:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("EXCEPTION");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
                case LogType.Failure:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(DateTime.Now.ToString(datetimeFormat));
                    Console.ForegroundColor = orgCol;
                    if (logOwner.Length > 0)
                    {
                        Console.Write(" (");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(logOwner);
                        Console.ForegroundColor = orgCol;
                        Console.Write(")");
                    }

                    Console.Write(" [");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("FAILURE");
                    Console.ForegroundColor = orgCol;
                    Console.Write("] ");
                    Console.Write(text);
                    Console.Write(Environment.NewLine);
                    break;
            }
        }
    }
}