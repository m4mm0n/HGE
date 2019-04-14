using System;
using System.Drawing;
using System.Drawing.Imaging;
using HGE.Events;

namespace HGE.Graphics
{
    public static class GraphicsExt
    {
        public static void Draw(this Graphics2DGL g, Point p, Pixel col)
        {
            g.Draw(p.X, p.Y, col);
        }

        public static void DrawLine(this Graphics2DGL g, Point p1, Point p2, Pixel col)
        {
            g.DrawLine(p1.X, p1.Y, p2.X, p2.Y, col);
        }

        public static void FillTriangle(this Graphics2DGL g, Point p1, Point p2, Point p3, Pixel col)
        {
            g.FillTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, col);
        }

        public static void DrawPath(this Graphics2DGL g, Point[] points, Pixel col)
        {
            for (var i = 0; i < points.Length - 1; i++)
                DrawLine(g, points[i], points[i + 1], col);
        }

        public static void DrawPolygon(this Graphics2DGL g, Point[] verts, Pixel col)
        {
            for (var i = 0; i < verts.Length - 1; i++)
                DrawLine(g, verts[i], verts[i + 1], col);
            DrawLine(g, verts[verts.Length - 1], verts[0], col);
        }

        public static void FillPolygon(this Graphics2DGL g, Point[] verts, Pixel col)
        {
            for (var i = 1; i < verts.Length - 1; i++)
                FillTriangle(g, verts[0], verts[i], verts[i + 1], col);
        }

        public static void DrawEllipse(this Graphics2DGL g, Point p, int width, int height, Pixel col)
        {
            if (width == 0 || height == 0)
                return;

            var a2 = width * width;
            var b2 = height * height;
            int fa2 = 4 * a2, fb2 = 4 * b2;
            int sigma;

            sigma = 2 * b2 + a2 * (1 - 2 * height);
            for (int x = 0, y = height; b2 * x <= a2 * y; x++)
            {
                g.Draw(p.X + x, p.Y + y, col);
                g.Draw(p.X - x, p.Y + y, col);
                g.Draw(p.X + x, p.Y - y, col);
                g.Draw(p.X - x, p.Y - y, col);

                if (sigma >= 0)
                    sigma += fa2 * (1 - y--);
                sigma += b2 * (4 * x + 6);
            }

            sigma = 2 * a2 + b2 * (1 - 2 * width);
            for (int x = width, y = 0; a2 * y <= b2 * x; y++)
            {
                g.Draw(p.X + x, p.Y + y, col);
                g.Draw(p.X - x, p.Y + y, col);
                g.Draw(p.X + x, p.Y - y, col);
                g.Draw(p.X - x, p.Y - y, col);

                if (sigma >= 0)
                    sigma += fb2 * (1 - x--);
                sigma += a2 * (4 * y + 6);
            }
        }

        public static void FillEllipse(this Graphics2DGL g, Point p, int width, int height, Pixel col)
        {
            if (width == 0 || height == 0)
                return;

            void ScanLine(int sx, int ex, int y)
            {
                for (var i = sx; i <= ex; i++)
                    g.Draw(i, y, col);
            }

            var a2 = width * width;
            var b2 = height * height;
            int fa2 = 4 * a2, fb2 = 4 * b2;
            int sigma;

            sigma = 2 * b2 + a2 * (1 - 2 * height);
            for (int x = 0, y = height; b2 * x <= a2 * y; x++)
            {
                ScanLine(p.X - x, p.X + x, p.Y - y);
                ScanLine(p.X - x, p.X + x, p.Y + y);

                if (sigma >= 0)
                    sigma += fa2 * (1 - y--);
                sigma += b2 * (4 * x + 6);
            }

            sigma = 2 * a2 + b2 * (1 - 2 * width);
            for (int x = width, y = 0; a2 * y <= b2 * x; y++)
            {
                ScanLine(p.X - x, p.X + x, p.Y - y);
                ScanLine(p.X - x, p.X + x, p.Y + y);

                if (sigma >= 0)
                    sigma += fb2 * (1 - x--);
                sigma += a2 * (4 * y + 6);
            }
        }

        public static void DrawArc(this Graphics2DGL g, Point p, int radius, Pixel col, int angle = 360)
        {
            for (var i = 0; i < angle; i++)
            {
                var x = (int) (radius * Math.Cos(i / 57.29577f));
                var y = (int) (radius * Math.Sin(i / 57.29577f));

                var v = new Point(p.X + x, p.Y + y);
                Draw(g, v, col);
            }
        }

        public static void FillArc(this Graphics2DGL g, Point p, int radius, int startAngle, int endAngle, Pixel col)
        {
            for (var i = startAngle; i > -endAngle + startAngle; i++)
            for (var j = 0; j < radius + 1; j++)
            {
                var x = (int) (j * Math.Cos(i / 57.29577f));
                var y = (int) (j * Math.Sin(i / 57.29577f));

                var v = new Point(p.X + x, p.Y + y);
                Draw(g, v, col);
            }
        }

        public static void DrawGrid(this Graphics2DGL g, Point a, Point b, int spacing, Pixel col)
        {
            for (var y = a.Y; y < b.Y / spacing; y++)
                DrawLine(g, new Point(a.X, y * spacing), new Point(b.X, y * spacing), col);
            for (var x = a.X; x < b.X / spacing; x++)
                DrawLine(g, new Point(x * spacing, a.Y), new Point(x * spacing, b.Y), col);
        }

        public static void DrawBitmap(this Graphics2DGL gfx, string fileName, bool useUnsafe = false)
        {
            DrawBitmap(gfx, new Bitmap(fileName), useUnsafe);
        }

        public static void DrawBitmap(this Graphics2DGL gfx, Bitmap imageToDraw, bool useUnsafe = false)
        {
            DrawBitmap(gfx, imageToDraw,
                new Rectangle(0, 0, -1, -1), new Rectangle(0, 0, -1, -1), useUnsafe);
        }

        public static void DrawBitmap(this Graphics2DGL gfx, Bitmap imageToDraw, Rectangle sourceRect,
            Rectangle destRect, bool useUnsafe = false)
        {
            try
            {
                var bWidth = sourceRect.Width != -1 ? sourceRect.Width : imageToDraw.Width;
                var bHeight = sourceRect.Height != -1 ? sourceRect.Height : imageToDraw.Height;
                int dWidth = 0, dHeight = 0;

                if (useUnsafe)
                {
                    var bmp = imageToDraw;

                    unsafe
                    {
                        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                        var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                        var scan0 = (byte*) bmpData.Scan0;
                        var depth = Image.GetPixelFormatSize(bmp.PixelFormat);
                        var length = bmp.Width * bmp.Height * depth / 8;

                        for (var x = sourceRect.X; x < bWidth; x++)
                        {
                            if (destRect.Width != -1)
                                if (dWidth >= destRect.Width)
                                    break;

                            for (var y = sourceRect.Y; y < bHeight; y++)
                            {
                                var i = (y * bmp.Width + x) * depth / 8;

                                var c = Color.Empty;

                                switch (depth)
                                {
                                    case 32:
                                    {
                                        var b = scan0[i];
                                        var g = scan0[i + 1];
                                        var r = scan0[i + 2];
                                        var a = scan0[i + 3];
                                        c = Color.FromArgb(a, r, g, b);
                                        break;
                                    }

                                    case 24:
                                    {
                                        var b = scan0[i];
                                        var g = scan0[i + 1];
                                        var r = scan0[i + 2];
                                        c = Color.FromArgb(r, g, b);
                                        break;
                                    }

                                    case 8:
                                    {
                                        var b = scan0[i];
                                        c = Color.FromArgb(b, b, b);
                                        break;
                                    }
                                }

                                var x0 = x - sourceRect.X;
                                var y0 = y - sourceRect.Y;

                                x0 += destRect.X;
                                y0 += destRect.Y;

                                if (destRect.Height > -1)
                                    if (dHeight >= destRect.Height)
                                        break;

                                gfx.Draw(x0, y0, new Pixel(c));

                                if (destRect.Height != -1)
                                    dHeight++;
                            }

                            if (destRect.Width != -1)
                                dWidth++;
                        }

                        bmp.UnlockBits(bmpData);
                    }
                }
                else
                {
                    for (var x = sourceRect.X; x < bWidth; x++)
                    {
                        if (destRect.Width != -1)
                            if (dWidth >= destRect.Width)
                                break;

                        for (var y = sourceRect.Y; y < bHeight; y++)
                        {
                            var x0 = x - sourceRect.X;
                            var y0 = y - sourceRect.Y;

                            x0 += destRect.X;
                            y0 += destRect.Y;

                            if (destRect.Height != -1)
                                if (dHeight >= destRect.Height)
                                    break;
                            var pixel = imageToDraw.GetPixel(x, y);
                            gfx.Draw(x0, y0, new Pixel(pixel.R, pixel.G, pixel.B));

                            if (destRect.Height != -1)
                                dHeight++;
                        }

                        if (destRect.Width != -1)
                            dWidth++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Graphics2DGLException(gfx, "GraphicsExt::DrawBitmap", ex);
            }
        }
    }
}