using HGE.Events;
using System;

namespace HGE.Graphics.Primitives
{
    internal class Circle : IPrimitive
    {
        public void Draw(Graphics2DGL core, params object[] parameters)
        {
            if(parameters.Length != 4)
                throw new Graphics2DGLException(core, "[Primitives] Circle - must have 4 parameters set!");

            var x0 = (int) parameters[0];
            var y0 = (int) parameters[1];
            var r = (int) parameters[2];
            var color = (Pixel) parameters[3];

            x0 += r - 1;
            y0 += r - 1;

            var x = r - 1;
            var y = 0;
            var dx = 1;
            var dy = 1;
            var err = dx - (r << 1);

            while (x >= y)
            {
                core.Draw(x0 + x, y0 + y, color);
                core.Draw(x0 + y, y0 + x, color);
                core.Draw(x0 - y, y0 + x, color);
                core.Draw(x0 - x, y0 + y, color);
                core.Draw(x0 - x, y0 - y, color);
                core.Draw(x0 - y, y0 - x, color);
                core.Draw(x0 + y, y0 - x, color);
                core.Draw(x0 + x, y0 - y, color);

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }

                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (r << 1);
                }
            }
        }
        public void Fill(Graphics2DGL core, params object[] parameters)
        {
            if (parameters.Length != 4)
                throw new Graphics2DGLException(core, "[Primitives] Circle - must have 4 parameters set!");

            var x = (int)parameters[0];
            var y = (int)parameters[1];
            var r = (int)parameters[2];
            var color = (Pixel)parameters[3];

            for (var i = 0; i < r * 2; i++)
            for (var j = 0; j < r * 2; j++)
            {
                var dist = Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j));
                if (dist < r) core.Draw(x - 1 + i, y - 1 + j, color);
            }
        }
    }
}
