using System;
using OpenTK.Graphics.OpenGL;

namespace HGE.Events
{
    public class ErrorEvent : EventArgs
    {
        public ErrorEvent(ErrorCode code, string format, params object[] param)
        {
            Code = code;
            ErrorMessage = "[" + DateTime.Now.ToShortTimeString() + "] " + string.Format(format, param);
        }

        public string ErrorMessage { get; set; }
        public ErrorCode Code { get; set; }
    }
}