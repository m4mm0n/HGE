using System;
using HGE.Events;
using HGE.Graphics;
using OpenTK.Graphics.OpenGL;

namespace HGE
{
    /// <summary>
    ///     Abstract form for more engines
    /// </summary>
    public abstract class IEngine : IDisposable
    {
        #region Logging

        public event EventHandler<LogEvent> OnLog;
        public event EventHandler<ErrorEvent> OnError;

        internal void Log(string format, params object[] args)
        {
            OnLog?.Invoke(this, new LogEvent(format, args));
        }

        internal void Error(ErrorCode error, string format, params object[] args)
        {
            OnError?.Invoke(this, new ErrorEvent(error, format, args));
        }

        #endregion

        #region Public Stuff

        public int MaxFPS;
        public int Width;
        public int HalfWidth;
        public int Height;
        public int HalfHeight;
        public bool IsVerbose;

        #endregion

        #region Overrideables

        public virtual void Clear(Pixel color)
        {
        }

        public virtual void Initialize(params object[] parameters)
        {
        }

        public virtual void Dispose()
        {
        }

        #endregion
    }
}