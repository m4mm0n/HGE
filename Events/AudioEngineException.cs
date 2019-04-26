using System;
using HGE.IO.AudioEngine;
using HGE.IO.AudioEngine.BASSlib;

namespace HGE.Events
{
    public class AudioEngineException : Exception
    {
        private readonly string ErrorMsg;
        private Exception innerException;
        private BASSError errorC;

        public AudioEngineException(int errorCode) : this(errorCode, string.Empty, null)
        { }

        public AudioEngineException(int errorCode, string errorMessage) : this(errorCode, errorMessage, null)
        { }

        public AudioEngineException(int errorCode, string errorMessage, Exception InnerEx)
        {
            ErrorMsg = errorMessage;
            errorC = (BASSError)errorCode;
            innerException = InnerEx;
        }

        public override string Message => string.Format("AudioEngine Error: {0}{1}(StackException: {2})", ErrorMsg,
            Environment.NewLine, base.Message);

        public override string ToString()
        {
            return string.Format("AudioEngine Error Info:{0}" +
                                 "------------{0}" +
                                 "Code       : {1}{0}" +
                                 "Message    : {2}{0}" +
                                 "Exception Info:{0}" +
                                 "---------------{0}" +
                                 "{3}",
                Environment.NewLine,
                errorC,
                ErrorMsg,
                base.ToString());
        }
    }
}
