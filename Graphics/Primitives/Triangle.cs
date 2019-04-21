using System;
using HGE.Events;

namespace HGE.Graphics.Primitives
{
    internal class Triangle : IPrimitive
    {
        public void Draw(Graphics2DGL core, params object[] parameters)
        {
            //int x1, int y1, int x2, int y2, int x3, int y3, Pixel color
            if (parameters.Length != 7)
                throw new Graphics2DGLException(core, "[Primitives] Triangle - must have 7 parameters set!");

            var x1 = (int) parameters[0];
            var y1 = (int) parameters[1];
            var x2 = (int) parameters[2];
            var y2 = (int) parameters[3];
            var x3 = (int) parameters[4];
            var y3 = (int) parameters[5];
            var color = (Pixel) parameters[6];

            core.DrawLine(x1, y1, x2, y2, color);
            core.DrawLine(x2, y2, x3, y3, color);
            core.DrawLine(x1, y1, x3, y3, color);
        }
        public void Fill(Graphics2DGL core, params object[] parameters)
        {
            if (parameters.Length != 7)
                throw new Graphics2DGLException(core, "[Primitives] Triangle - must have 7 parameters set!");

            var x1 = (int)parameters[0];
            var y1 = (int)parameters[1];
            var x2 = (int)parameters[2];
            var y2 = (int)parameters[3];
            var x3 = (int)parameters[4];
            var y3 = (int)parameters[5];
            var color = (Pixel)parameters[6];

            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            for (var x = minX; x < maxX; x++)
            for (var y = minY; y < maxY; y++)
            {
                float d1, d2, d3;
                bool hasNeg, hasPos;

                d1 = core.Sign(x, y, x1, y1, x2, y2);
                d2 = core.Sign(x, y, x2, y2, x3, y3);
                d3 = core.Sign(x, y, x3, y3, x1, y1);

                hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
                hasPos = d1 > 0 || d2 > 0 || d3 > 0;

                if (!(hasNeg && hasPos)) core.Draw(x, y, color);
            }
        }
    }
}
