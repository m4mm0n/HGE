using System;
using HGE.Graphics;

namespace HGE.Events
{
    public class Graphics2DGLException : Exception
    {
        private readonly string ErrorMsg;
        private readonly Graphics2DGL graphics;
        private Exception innerException;

        public Graphics2DGLException(Graphics2DGL gfx) : this(gfx, string.Empty, null)
        {
        }

        public Graphics2DGLException(Graphics2DGL gfx, Exception innerEx) : this(gfx, string.Empty, innerEx)
        {
        }

        public Graphics2DGLException(Graphics2DGL gfx, string errorMsg) : this(gfx, errorMsg, null)
        {
        }

        public Graphics2DGLException(Graphics2DGL gfx, string errorMsg, Exception innerEx)
        {
            graphics = gfx;
            ErrorMsg = errorMsg;
            innerException = innerEx;
        }

        public override string Message => string.Format("Graphics2D GL Error: {0}{1}(StackException: {2})", ErrorMsg,
            Environment.NewLine, base.Message);

        public override string ToString()
        {
            return string.Format("Graphics2D GL Info:{0}" +
                                 "-------------------{0}" +
                                 "DrawTarget Sprite : {1}{0}" +
                                 "Opacity Mode      : {2}{0}" +
                                 "Exception Info    : {3}",
                Environment.NewLine,
                graphics?.DrawTarget,
                graphics?.OpacityMode.ToString(),
                base.ToString());
        }
    }
}