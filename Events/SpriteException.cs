using System;
using HGE.Graphics;

namespace HGE.Events
{
    public class SpriteException : Exception
    {
        private readonly string ErrorMsg;

        private readonly string sH;
        private readonly string sP;
        private readonly Sprite sprite;
        private readonly string sW;
        private Exception innerException;

        public SpriteException(Sprite sp) : this(sp, string.Empty, null)
        {
        }

        public SpriteException(Sprite sp, string errorMsg) : this(sp, errorMsg, null)
        {
        }

        public SpriteException(Sprite sp, string errorMsg, Exception innerEx)
        {
            sprite = sp;
            innerException = innerEx;
            ErrorMsg = errorMsg;

            sH = sprite.Height > 0 ? sprite.Height.ToString() : "N/A";
            sW = sprite.Width > 0 ? sprite.Width.ToString() : "N/A";
            sP = sprite.Pixels?.Length > 0 ? sprite.Pixels.Length.ToString() : "N/A";
        }

        public override string Message => string.Format("Sprite Error: {0}{1}(StackException: {2})", ErrorMsg,
            Environment.NewLine, base.Message);

        public override string ToString()
        {
            return string.Format("Sprite Info:{0}" +
                                 "------------{0}" +
                                 "Height     : {1}{0}" +
                                 "Width      : {2}{0}" +
                                 "SampleMode : {3}{0}" +
                                 "Pixel Count: {4}{0}{0}" +
                                 "Exception Info:{0}" +
                                 "---------------{0}" +
                                 "{5}",
                Environment.NewLine,
                sH,
                sW,
                sprite?.SampleMode.ToString(),
                sP,
                base.ToString());
        }
    }
}