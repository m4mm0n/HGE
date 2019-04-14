using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace HGE.Graphics
{
    public struct Pixel
    {
        public byte red, green, blue, alpha;

        /// <summary>
        ///     The Constructor for a pixel in RGBA form.
        /// </summary>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        /// <param name="alpha">Alpha</param>
        public Pixel(byte red, byte green, byte blue, byte alpha = 255)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }

        public Pixel(Color col) : this(col.R, col.G, col.B, col.A)
        {
        }

        public Pixel(int red, int green, int blue, int alpha = 255) :
            this((byte) red, (byte) green, (byte) blue, (byte) alpha)
        {
        }

        #region Constants

        // Greyscale Colors
        public static Pixel BLANK = new Pixel(0, 0, 0, 0);
        public static Pixel WHITE = new Pixel(255, 255, 255);
        public static Pixel GREY = new Pixel(192, 192, 192);
        public static Pixel BLACK = new Pixel(0, 0, 0);

        public static Pixel DARK_GREY = new Pixel(128, 128, 128);

        public static Pixel VERY_DARK_GREY = new Pixel(64, 64, 64);

        // RGB Colors
        public static Pixel RED = new Pixel(255, 0, 0);
        public static Pixel GREEN = new Pixel(0, 255, 0);
        public static Pixel BLUE = new Pixel(0, 0, 255);

        public static Pixel DARK_RED = new Pixel(128, 0, 0);
        public static Pixel DARK_GREEN = new Pixel(0, 128, 0);
        public static Pixel DARK_BLUE = new Pixel(0, 0, 128);

        public static Pixel VERY_DARK_RED = new Pixel(64, 0, 0);
        public static Pixel VERY_DARK_GREEN = new Pixel(0, 64, 0);
        public static Pixel VERY_DARK_BLUE = new Pixel(0, 0, 64);

        // CYM Colors
        public static Pixel YELLOW = new Pixel(255, 255, 0);
        public static Pixel MAGENTA = new Pixel(255, 0, 255);
        public static Pixel CYAN = new Pixel(0, 255, 255);

        public static Pixel DARK_YELLOW = new Pixel(128, 128, 0);
        public static Pixel DARK_MAGENTA = new Pixel(128, 0, 128);
        public static Pixel DARK_CYAN = new Pixel(0, 128, 128);

        public static Pixel VERY_DARK_YELLOW = new Pixel(64, 64, 0);
        public static Pixel VERY_DARK_MAGENTA = new Pixel(64, 0, 64);
        public static Pixel VERY_DARK_CYAN = new Pixel(0, 64, 64);

        #endregion

        /// <summary>
        ///     Interpolates two pixels
        /// </summary>
        /// <param name="first">First Pixel</param>
        /// <param name="second">Second Pixel</param>
        /// <param name="value">Mix Value (0-1)</param>
        /// <returns>The Blended Pixel</returns>
        public static Pixel Lerp(Pixel first, Pixel second, float value)
        {
            var red = (byte) ((1 - value) * first.red + value * second.red);
            var green = (byte) ((1 - value) * first.green + value * second.green);
            var blue = (byte) ((1 - value) * first.blue + value * second.blue);
            var alpha = (byte) ((1 - value) * first.alpha + value * second.alpha);

            return new Pixel(red, green, blue, alpha);
        }

        public static Pixel SmoothStep(Pixel first, Pixel second, float amount)
        {
            amount = amount * amount * (3 - 2 * amount);
            var red = first.red + (second.red - first.red) * amount;
            var green = first.green + (second.green - first.green) * amount;
            var blue = first.blue + (second.blue - first.blue) * amount;

            return new Pixel((byte) red, (byte) green, (byte) blue);
        }

        /// <summary>
        ///     Compares this pixel's color to another
        /// </summary>
        /// <param name="other">The other pixel</param>
        /// <returns>If both colors are the same</returns>
        public bool Compare(Pixel other)
        {
            return other.red == red && other.green == green &&
                   other.blue == blue && other.alpha == alpha;
        }

        public override string ToString()
        {
            return string.Format("R: {0}, G: {1}, B: {2}, Alpha: {3}", red.ToString("X"), green.ToString("X"),
                blue.ToString("X"), alpha.ToString("X"));
        }

        public Color ToSystemColor => Color.FromArgb(alpha, red, green, blue);

        public Pixel Darken(float percent)
        {
            return new Pixel(ControlPaint.Dark(ToSystemColor, percent));
        }

        public Pixel Lighten(float percent)
        {
            return new Pixel(ControlPaint.Light(ToSystemColor, percent));
        }

        public static Pixel FromRgb(uint rgb)
        {
            //var argb = (alpha << 24) + (red << 16) + (green << 8) + blue;

            //(red << 24) + (green << 16) + (blue << 8) + alpha
            var a = (byte) (rgb & 0xFF);
            var b = (byte) ((rgb >> 8) & 0xFF);
            var g = (byte) ((rgb >> 16) & 0xFF);
            var r = (byte) ((rgb >> 24) & 0xFF);

            return new Pixel(r, g, b, a);
        }

        public static Pixel FromHsv(float h, float s, float v)
        {
            var c = v * s;
            var nh = h / 60 % 6;
            var x = c * (1 - Math.Abs(nh % 2 - 1));
            var m = v - c;

            float r, g, b;

            if (0 <= nh && nh < 1)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (1 <= nh && nh < 2)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (2 <= nh && nh < 3)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (3 <= nh && nh < 4)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (4 <= nh && nh < 5)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (5 <= nh && nh < 6)
            {
                r = c;
                g = 0;
                b = x;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }

            r += m;
            g += m;
            b += m;

            return new Pixel((byte) Math.Floor(r * 255), (byte) Math.Floor(g * 255), (byte) Math.Floor(b * 255));
        }

        public static Pixel[] ToPixels(int[] pixelArray)
        {
            var tmp = new List<Pixel>();
            foreach (var i in pixelArray) tmp.Add(ToPixel((uint) i));

            return tmp.ToArray();
        }

        public static Pixel[] ToPixels(uint[] pixelArray)
        {
            var tmp = new List<Pixel>();
            foreach (var u in pixelArray) tmp.Add(ToPixel(u));

            return tmp.ToArray();
        }

        public static uint[] ToUIntArray(Pixel[] pixels)
        {
            var tmp = new List<uint>();
            foreach (var p in pixels) tmp.Add(ToUInt(p));

            return tmp.ToArray();
        }

        public static Pixel ToPixel(uint i)
        {
            return new Pixel((byte) (i >> 0), (byte) (i >> 8),
                (byte) (i >> 16), (byte) (i >> 24));
        }

        public static uint ToUInt(Pixel p)
        {
            return (uint) ((p.alpha << 24) | (p.red << 16) | (p.green << 8) | (p.blue << 0));
        }

        public static Pixel ToPixel(string hexCode)
        {
            return new Pixel((byte) int.Parse(hexCode.Substring(0, 2), NumberStyles.HexNumber),
                (byte) int.Parse(hexCode.Substring(2, 2), NumberStyles.HexNumber),
                (byte) int.Parse(hexCode.Substring(4, 2), NumberStyles.HexNumber));
        }

        // -- Operators --

        public static Pixel operator +(Pixel f, Pixel p)
        {
            return new Pixel((byte) (f.red + p.red), (byte) (f.green + p.green),
                (byte) (f.blue + p.blue), (byte) (f.alpha + p.alpha));
        }

        public static Pixel operator -(Pixel f, Pixel p)
        {
            return new Pixel((byte) (f.red - p.red), (byte) (f.green - p.green),
                (byte) (f.blue - p.blue), (byte) (f.alpha - p.alpha));
        }

        public static Pixel operator *(Pixel f, float t)
        {
            return new Pixel((byte) (f.red * t), (byte) (f.green * t),
                (byte) (f.blue * t), (byte) (f.alpha * t));
        }

        public static bool operator ==(Pixel a, Pixel b)
        {
            return a.Compare(b);
        }

        public static bool operator !=(Pixel a, Pixel b)
        {
            return !a.Compare(b);
        }
    }
}