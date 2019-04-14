using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HGE.Graphics;

namespace HGE.IO
{
    public static class Helpers
    {
        private static readonly Random _rand;

        static Helpers()
        {
            _rand = new Random();
        }

        [DllImport("kernel32.dll")]
        public static extern void Sleep(int dwMilliseconds);

        public static TimeSpan GetTimeSpan(this int toGetFrom, int interval)
        {
            var ts = new TimeSpan();
            if (interval == 1000)
                ts = TimeSpan.FromSeconds(toGetFrom);
            else if (interval < 1000)
                ts = TimeSpan.FromMilliseconds(toGetFrom * interval);

            return ts;
        }

        public static Bitmap ResizeImage(Bitmap image, int Width, int Height, bool preserveAspectRatio)
        {
            return ResizeImage(image, new Size(Width, Height), preserveAspectRatio);
        }

        public static Bitmap ResizeImage(Bitmap image, Size size,
            bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                var originalWidth = image.Width;
                var originalHeight = image.Height;
                var percentWidth = size.Width / (float) originalWidth;
                var percentHeight = size.Height / (float) originalHeight;
                var percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int) (originalWidth * percent);
                newHeight = (int) (originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphicsHandle = System.Drawing.Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static int FloatToScalar(float value)
        {
            return (int) Math.Ceiling(value * 128f);
        }

        public static int rand()
        {
            return _rand.Next();
        }

        public static int rand(int max)
        {
            return _rand.Next(max);
        }

        public static int rand(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public static float GetPercentage(int current, int total)
        {
            var x = (float) current / total;
            return x;
        }

        public static float GetPercentage(float current, float total)
        {
            return current / total * 100f;
        }

        public static double GetPercentage(double current, double total)
        {
            return current / total;
        }

        public static double Percent(this double number, int percent)
        {
            //return ((double) 80         *       25)/100;
            return number * percent / 100;
        }

        public static double Percent(this double number, double percent)
        {
            return number * percent / 100.0;
        }

        public static SizeF MeasureTextSize(string text, Font font)
        {
            using (var a = new Bitmap(512, 512))
            using (var b = System.Drawing.Graphics.FromImage(a))
            {
                return b.MeasureString(text, font);
            }
        }

        public static int Doubler(this double val)
        {
            return (int) (val * 1000);
        }

        public static PrivateFontCollection LoadFontFromFile(string fileName)
        {
            if (File.Exists(fileName))
                return LoadFontFromMemory(File.ReadAllBytes(fileName));

            throw new FileNotFoundException();
        }

        public static PrivateFontCollection LoadFontFromMemory(byte[] fontBytes)
        {
            var data = Marshal.AllocCoTaskMem(fontBytes.Length);
            var fnt = new PrivateFontCollection();

            Marshal.Copy(fontBytes, 0, data, fontBytes.Length);
            fnt.AddMemoryFont(data, fontBytes.Length);
            Marshal.FreeCoTaskMem(data);
            return fnt;
        }

        public static PrivateFontCollection LoadFontFromMemory(Stream fontStream)
        {
            var bytes = new byte[fontStream.Length];
            fontStream.Read(bytes, 0, bytes.Length);
            return LoadFontFromMemory(bytes);
        }

        public static byte[] GetBytes(this Stream strm)
        {
            byte[] result;
            try
            {
                using (var mstream = new MemoryStream())
                {
                    strm.CopyTo(mstream);
                    result = mstream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[GetBytes] FAIL!", ex);
            }

            return result;
        }

        public static byte[] FindEmbedded(this Assembly asm, string nameToFind)
        {
            byte[] bytes;
            try
            {
                bytes = asm.GetManifestResourceStream(asm.GetManifestResourceNames().First(x => x.EndsWith(nameToFind)))
                    .GetBytes();
            }
            catch (Exception ex)
            {
                throw new Exception("[FindEmbedded] FAIL!", ex);
            }

            return bytes;
        }

        public static Bitmap ToBitmap(this string toConvert, Font font, Pixel textColor, Pixel background,
            int size = 18)
        {
            var bmp = new Bitmap(1, 1);

            var gfx = System.Drawing.Graphics.FromImage(bmp);
            var fnt = new Font(font.FontFamily, size);
            var strSize = gfx.MeasureString(toConvert, fnt);

            bmp = new Bitmap(bmp, (int) strSize.Width, (int) strSize.Height);
            gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.FillRectangle(new SolidBrush(background.ToSystemColor), 0, 0, bmp.Width, bmp.Height);
            gfx.DrawString(toConvert, fnt, new SolidBrush(textColor.ToSystemColor), 0, 0);
            fnt.Dispose();
            gfx.Flush();
            gfx.Dispose();

            return bmp;
        }

        public static double DoubleParseAdvanced(this string strToParse, char decimalSymbol = ',')
        {
            var tmp = Regex.Match(strToParse,
                @"([-]?[0-9]+)([\s])?([0-9]+)?[." + decimalSymbol + "]?([0-9 ]+)?([0-9]+)?").Value;

            if (tmp.Length > 0 && strToParse.Contains(tmp))
            {
                var currDecSeparator = Application.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                tmp = tmp.Replace(".", currDecSeparator).Replace(decimalSymbol.ToString(), currDecSeparator);

                return double.Parse(tmp);
            }

            return 0;
        }
    }
}