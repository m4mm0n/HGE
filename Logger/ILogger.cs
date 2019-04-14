using System;

namespace HGE.Logger
{
    public interface ILogger
    {
        void Success(string format, params object[] args);
        void Failure(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
        void Exception(string format, params object[] args);
        void Exception(Exception ex);
        void Critical(string format, params object[] args);
        void Normal(string format, params object[] args);
        void Verbose(string format, params object[] args);
#if DEBUG
        void Debug(string format, params object[] args);
#endif
    }
}