using System;
using HGE.Graphics.Primitives;

namespace HGE.Graphics
{
    public enum OpacityMode
    {
        NORMAL,
        MASK,
        ALPHA
    }

    public class Graphics2DGL
    {
        private Sprite fontSprite;

        public Graphics2DGL(int width, int height)
        {
            DrawTarget = new Sprite(width, height);
            ConstructFontSheet();
        }

        /// <summary>
        ///     The Alpha 'Blending' Modes
        ///     Normal - Alpha has no Affect
        ///     Mask - Any Alpha below 255 doesn't get rendered
        ///     Alpha - 'Proper' Alpha Blending
        /// </summary>
        public OpacityMode OpacityMode { get; set; } = OpacityMode.ALPHA;

        /// <summary>
        ///     The current frame or sprite that is being drawn on
        /// </summary>
        public Sprite DrawTarget { get; set; }

        /// <summary>
        ///     Draws a pixel on the screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Color</param>
        public virtual void Draw(int x, int y, Pixel color)
        {
            if (color.alpha < 255)
            {
                if (OpacityMode == OpacityMode.MASK)
                    return;

                if (OpacityMode == OpacityMode.ALPHA)
                {
                    var l = Pixel.Lerp(DrawTarget.GetPixel(x, y), color, color.alpha / 255f);
                    DrawTarget.SetPixel(x, y, new Pixel(l.red, l.green, l.blue));
                    return;
                }
            }

            DrawTarget.SetPixel(x, y, color);
        }

        /// <summary>
        ///     Draw specific primitive to screen
        /// </summary>
        /// <param name="primitive">Primitive Type</param>
        /// <param name="fill">Fills the screen</param>
        /// <param name="parameters">The needed parameters</param>
        public virtual void Draw(DrawablePrimitives primitive, bool fill, params object[] parameters)
        {
            switch (primitive)
            {
                case DrawablePrimitives.Circle:
                    if(fill)
                        new Circle().Fill(this, parameters);
                    else
                        new Circle().Draw(this, parameters);
                    break;
                case DrawablePrimitives.Rectangle:
                    if(fill)
                        new Rectangle().Fill(this, parameters);
                    else
                        new Rectangle().Draw(this, parameters);
                    break;
                case DrawablePrimitives.Triangle:
                    if(fill)
                        new Triangle().Fill(this, parameters);
                    else
                        new Triangle().Draw(this, parameters);
                    break;
            }
        }

        /// <summary>
        ///     Draws a line
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="color">Color</param>
        /// <param name="thickness">Thickness</param>
        public void DrawLine(int x1, int y1, int x2, int y2, Pixel color, int thickness = 1)
        {
            float x, y, step;
            float dx = x2 - x1;
            float dy = y2 - y1;

            var absDX = Math.Abs(dx);
            var absDY = Math.Abs(dy);

            step = absDX >= absDY ? absDX : absDY;

            dx /= step;
            dy /= step;

            x = x1;
            y = y1;

            for (var i = 1; i <= step; i++)
            {
                Draw((int) x, (int) y, color);

                if (thickness > 1)
                    for (var j = 1; j < thickness; j++)
                    {
                        Draw((int) x + j, (int) y, color);
                        Draw((int) x, (int) y + j, color);
                    }

                x += dx;
                y += dy;
            }
        }

        /// <summary>
        ///     Draws a rectangle outline
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void DrawRect(int x, int y, int w, int h, Pixel color)
        {
            if (w < 0)
            {
                w *= -1;
                x -= w;
            }

            DrawLine(x, y, x + w, y, color);
            DrawLine(x + w - 1, y, x + w - 1, y + h, color);
            DrawLine(x, y + h - 1, x + w, y + h - 1, color);
            DrawLine(x, y, x, y + h, color);
        }

        /// <summary>
        ///     Draws a filled In rectangle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void FillRect(int x, int y, int w, int h, Pixel color)
        {
            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
                Draw(x + i, y + j, color);
        }

        /// <summary>
        ///     Draws a circle outline
        /// </summary>
        /// <param name="x0">X</param>
        /// <param name="y0">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void DrawCircle(int x0, int y0, int r, Pixel color)
        {
            x0 += r - 1;
            y0 += r - 1;

            var x = r - 1;
            var y = 0;
            var dx = 1;
            var dy = 1;
            var err = dx - (r << 1);

            while (x >= y)
            {
                Draw(x0 + x, y0 + y, color);
                Draw(x0 + y, y0 + x, color);
                Draw(x0 - y, y0 + x, color);
                Draw(x0 - x, y0 + y, color);
                Draw(x0 - x, y0 - y, color);
                Draw(x0 - y, y0 - x, color);
                Draw(x0 + y, y0 - x, color);
                Draw(x0 + x, y0 - y, color);

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

        /// <summary>
        ///     Draws a filled in circle
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="r">Radius</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void FillCircle(int x, int y, int r, Pixel color)
        {
            for (var i = 0; i < r * 2; i++)
            for (var j = 0; j < r * 2; j++)
            {
                var dist = Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j));
                if (dist < r) Draw(x - 1 + i, y - 1 + j, color);
            }
        }

        /// <summary>
        ///     Draw a triangle outline
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel color)
        {
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x3, y3, color);
            DrawLine(x1, y1, x3, y3, color);
        }

        /// <summary>
        ///     Draws a filled in triangle
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <param name="x3">X3</param>
        /// <param name="y3">Y3</param>
        /// <param name="color">Color</param>
        [Obsolete("This method will be removed in favour of Draw(DrawablePrimitives, bool, params)!")]
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel color)
        {
            var minX = Math.Min(Math.Min(x1, x2), x3);
            var maxX = Math.Max(Math.Max(x1, x2), x3);

            var minY = Math.Min(Math.Min(y1, y2), y3);
            var maxY = Math.Max(Math.Max(y1, y2), y3);

            for (var x = minX; x < maxX; x++)
            for (var y = minY; y < maxY; y++)
            {
                float d1, d2, d3;
                bool hasNeg, hasPos;

                d1 = Sign(x, y, x1, y1, x2, y2);
                d2 = Sign(x, y, x2, y2, x3, y3);
                d3 = Sign(x, y, x3, y3, x1, y1);

                hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
                hasPos = d1 > 0 || d2 > 0 || d3 > 0;

                if (!(hasNeg && hasPos)) Draw(x, y, color);
            }
        }

        internal float Sign(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return (x1 - x3) * (y2 - y3) - (x2 - x3) * (y1 - y3);
        }

        /// <summary>
        ///     Draws a sprite on the screen
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="scale">Scale</param>
        /// <param name="sprite">The Sprite</param>
        public void DrawSprite(int x, int y, Sprite sprite, int scale = 1)
        {
            if (sprite == null || scale <= 0) return;
            for (var i = 0; i < sprite.Width; i++)
            for (var j = 0; j < sprite.Height; j++)
                FillRect((x + i) * scale, (y + j) * scale,
                    scale, scale, sprite.GetPixel(i, j));
        }

        /// <summary>
        ///     Draws a sprite on the screen with rotation
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="sprite">The Sprite</param>
        /// <param name="angle">Angle</param>
        public void DrawSprite(int x, int y, Sprite sprite, double angle)
        {
            if (sprite == null) return;

            var hWidth = sprite.Width / 2;
            var hHeight = sprite.Height / 2;

            var sin = Math.Sin(-angle);
            var cos = Math.Cos(-angle);

            for (var i = 0; i < sprite.Width * 2f; i++)
            for (var j = 0; j < sprite.Height * 2f; j++)
            {
                var xt = i - sprite.Width;
                var yt = j - sprite.Height;

                var xs = (int) Math.Round(cos * xt - sin * yt) + hWidth;
                var ys = (int) Math.Round(sin * xt + cos * yt) + hHeight;

                var pixel = sprite.GetPixel(xs, ys);

                if (!pixel.Compare(new Pixel(0, 0, 0, 0)))
                    Draw(x + i - sprite.Width, y + j - sprite.Height, pixel);
            }
        }

        /// <summary>
        ///     Partially draws a sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="sprite">The Sprite</param>
        /// <param name="ox">Offset X</param>
        /// <param name="oy">Offset Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h)
        {
            if (sprite == null) return;
            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
                Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy));
        }

        /// <summary>
        ///     Draws a string using embeded character data.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="text">Text</param>
        /// <param name="color">Color</param>
        /// <param name="scale">Scale</param>
        public void DrawString(int x, int y, string text, Pixel color, int scale = 1)
        {
            var sx = 0;
            var sy = 0;
            foreach (var c in text)
                if (c == '\n')
                {
                    sx = 0;
                    sy += 8 * scale;
                }
                else
                {
                    var ox = (c - 32) % 16;
                    var oy = (c - 32) / 16;

                    if (scale > 1)
                    {
                        for (var i = 0; i < 8; i++)
                        for (var j = 0; j < 8; j++)
                            if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).red > 0)
                                for (var s = 0; s < scale; s++)
                                for (var js = 0; js < scale; js++)
                                    Draw(x + sx + i * scale + s, y + sy + j * scale + js, color);
                    }
                    else
                    {
                        for (var i = 0; i < 8; i++)
                        for (var j = 0; j < 8; j++)
                            if (fontSprite.GetPixel(i + ox * 8, j + oy * 8).red > 0)
                                Draw(x + sx + i, y + sy + j, color);
                    }

                    sx += 8 * scale;
                }
        }

        private void ConstructFontSheet()
        {
            var data = "";
            data += "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000";
            data += "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400";
            data += "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000";
            data += "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000";
            data += "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000";
            data += "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000";
            data += "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000";
            data += "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000";
            data += "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000";
            data += "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000";
            data += "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000";
            data += "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000";
            data += "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000";
            data += "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0";
            data += "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000";
            data += "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020";

            fontSprite = new Sprite(128, 48);
            int px = 0, py = 0;
            for (var b = 0; b < 1024; b += 4)
            {
                var sym1 = (uint) data[b + 0] - 48;
                var sym2 = (uint) data[b + 1] - 48;
                var sym3 = (uint) data[b + 2] - 48;
                var sym4 = (uint) data[b + 3] - 48;
                var r = (sym1 << 18) | (sym2 << 12) | (sym3 << 6) | sym4;

                for (var i = 0; i < 24; i++)
                {
                    var k = (byte) ((r & (1 << i)) > 0 ? 255 : 0);
                    fontSprite.SetPixel(px, py, new Pixel(k, k, k, k));
                    if (++py == 48)
                    {
                        px++;
                        py = 0;
                    }
                }
            }
        }
    }
}