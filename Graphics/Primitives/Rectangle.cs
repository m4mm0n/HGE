using HGE.Events;

namespace HGE.Graphics.Primitives
{
    internal class Rectangle : IPrimitive
    {
        public void Draw(Graphics2DGL core, params object[] parameters)
        {
            //int x, int y, int w, int h, Pixel color
            if (parameters.Length != 5)
                throw new Graphics2DGLException(core, "[Primitives] Circle - must have 5 parameters set!");

            var x = (int) parameters[0];
            var y = (int) parameters[1];
            var w = (int) parameters[2];
            var h = (int) parameters[3];
            var color = (Pixel) parameters[4];

            if (w < 0)
            {
                w *= -1;
                x -= w;
            }

            core.DrawLine(x, y, x + w, y, color);
            core.DrawLine(x + w - 1, y, x + w - 1, y + h, color);
            core.DrawLine(x, y + h - 1, x + w, y + h - 1, color);
            core.DrawLine(x, y, x, y + h, color);
        }
        public void Fill(Graphics2DGL core, params object[] parameters)
        {
            if (parameters.Length != 5)
                throw new Graphics2DGLException(core, "[Primitives] Circle - must have 5 parameters set!");

            var x = (int)parameters[0];
            var y = (int)parameters[1];
            var w = (int)parameters[2];
            var h = (int)parameters[3];
            var color = (Pixel)parameters[4];

            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
                core.Draw(x + i, y + j, color);
        }
    }
}
