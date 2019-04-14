using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using HGE.Events;

namespace HGE.Graphics
{
    public class Sprite : ICloneable, IDisposable
    {
        /// <summary>
        ///     SampleMode for the Sprite
        /// </summary>
        public OpacityMode SampleMode = OpacityMode.NORMAL;

        /// <summary>
        ///     Constructor for a blank sprite
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Sprite(int width, int height)
        {
            try
            {
                Width = width;
                Height = height;
                Pixels = new Pixel[width * height];
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::New", ex);
            }
        }

        /// <summary>
        ///     Constructor to make a hard copy of another sprite
        /// </summary>
        /// <param name="other"></param>
        public Sprite(Sprite other)
        {
            try
            {
                if (other == null) return;
                Pixels = new Pixel[other.Width * other.Height];
                for (var i = 0; i < other.Pixels.Length; i++)
                    Pixels[i] = other.Pixels[i];
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::New", ex);
            }
        }

        /// <summary>
        ///     Constructor for a sprite that will
        ///     be retrieved from a file on the computer
        /// </summary>
        /// <param name="path">The Path on the Computer</param>
        /// <param name="useUnsafe">[Optional] Some images may have errors on unmanaged unsafe loading!</param>
        public Sprite(string path, bool useUnsafe = false) : this(new Bitmap(path), useUnsafe)
        {
        }

        /// <summary>
        ///     Creates a new Sprite from an existing image
        /// </summary>
        /// <param name="img"></param>
        public Sprite(Image img) : this((Bitmap) img)
        {
        }

        /// <summary>
        ///     Creates a new Sprite from an existing bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <param name="useUnsafe">[Optional] Some images may have errors on unmanaged unsafe loading!</param>
        public Sprite(Bitmap image, bool useUnsafe = false)
        {
            try
            {
                Width = image.Width;
                Height = image.Height;

                Pixels = new Pixel[Width * Height];

                if (!useUnsafe)
                    for (var x = 0; x < image.Width; x++)
                    for (var y = 0; y < image.Height; y++)
                    {
                        var p = image.GetPixel(x, y);
                        var col = new Pixel(p.R, p.G, p.B, p.A);
                        SetPixel(x, y, col);
                    }
                else
                    unsafe
                    {
                        var rect = new Rectangle(0, 0, image.Width, image.Height);
                        var bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);

                        var scan0 = (byte*) bmpData.Scan0;
                        var depth = Image.GetPixelFormatSize(image.PixelFormat);

                        var length = Width * Height * depth / 8;

                        for (var x = 0; x < Width; x++)
                        for (var y = 0; y < Height; y++)
                        {
                            var i = (y * Width + x) * depth / 8;

                            switch (depth)
                            {
                                case 32:
                                {
                                    var b = scan0[i];
                                    var g = scan0[i + 1];
                                    var r = scan0[i + 2];
                                    var a = scan0[i + 3];
                                    this[x, y] = new Pixel(r, g, b, a);
                                    break;
                                }

                                case 24:
                                {
                                    var b = scan0[i];
                                    var g = scan0[i + 1];
                                    var r = scan0[i + 2];
                                    this[x, y] = new Pixel(r, g, b);
                                    break;
                                }

                                case 8:
                                {
                                    var b = scan0[i];
                                    this[x, y] = new Pixel(b, b, b);
                                    break;
                                }
                            }
                        }

                        image.UnlockBits(bmpData);
                    }
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::New", ex);
            }
        }

        /// <summary>
        ///     Gets the width of the Sprite
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Gets the height of the Sprite
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     Gets/Sets the Pixel-array
        /// </summary>
        public Pixel[] Pixels { get; set; }

        /// <summary>
        ///     Gets/Sets a Pixel at given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Pixel this[int x, int y]
        {
            get => GetPixel(x, y);
            set => SetPixel(x, y, value);
        }

        /// <summary>
        ///     Clones the Sprite 1:1
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            try
            {
                return new Sprite(this);
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::Clone", ex);
            }
        }

        /// <summary>
        ///     Disposes and collects the garbage from the Sprite
        /// </summary>
        public void Dispose()
        {
            try
            {
                Pixels = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::Dispose", ex);
            }
        }

        /// <summary>
        ///     Retrieves a pixel from the sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>A Pixel</returns>
        public Pixel GetPixel(int x, int y)
        {
            try
            {
                if (SampleMode == OpacityMode.NORMAL)
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                        return Pixels[y * Width + x];
                    else
                        return new Pixel();
                return Pixels[Math.Abs(y % Height) * Width + Math.Abs(x % Width)];
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::GetPixel", ex);
            }
        }

        /// <summary>
        ///     Sets a pixel in the sprite
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="color">Pixel Replacement</param>
        public void SetPixel(int x, int y, Pixel p)
        {
            try
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                    Pixels[y * Width + x] = p;
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::SetPixel", ex);
            }
        }

        /// <summary>
        ///     Clears the sprite to a specific color
        /// </summary>
        /// <param name="p">Pixel / Color</param>
        public void Clear(Pixel p)
        {
            try
            {
                for (var i = 0; i < Pixels.Length; i++)
                    Pixels[i] = p;
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::Clear", ex);
            }
        }

        /// <summary>
        ///     Clears the sprite with a blank color (R: 0, G: 0, B: 0, Alpha: 0)
        /// </summary>
        public void Clear()
        {
            Clear(Pixel.BLANK);
        }

        /// <summary>
        ///     Copies all pixels into another sprite
        ///     (Both sprites must be the same size!)
        /// </summary>
        /// <param name="destination">The Sprite to Copy To</param>
        public void CopyTo(Sprite destination)
        {
            if (destination == null)
                throw new SpriteException(this, "Sprite::CopyTo", new NullReferenceException());
            //throw new NullReferenceException();

            if (destination.Width != Width || destination.Height != Height)
                throw new SpriteException(this, "Sprite::CopyTo",
                    new Exception("Destination sprite size is not the same as the source sprite!"));
            //throw new Exception("Destination sprite size is not the same" +
            //                    "as the source sprite!");

            try
            {
                Pixels.CopyTo(destination.Pixels, 0);
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::CopyTo", ex);
            }
        }

        /// <summary>
        ///     Gets a sample of the Sprite's pixels
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Pixel Sample(float x, float y)
        {
            try
            {
                var sx = Math.Min((int) (x * Width), Width - 1);
                var sy = Math.Min((int) (y * Height), Height - 1);
                return GetPixel(sx, sy);
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::Sample", ex);
            }
        }

        /// <summary>
        ///     Fills a rectangle inside the Sprite
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="color"></param>
        public void FillRect(int x, int y, int w, int h, Pixel color)
        {
            try
            {
                for (var i = 0; i < w; i++)
                for (var j = 0; j < h; j++)
                    SetPixel(x + i, y + j, color);
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::FillRect", ex);
            }
        }

        /// <summary>
        ///     Gets a rectangle of pixels from the Sprite
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public Pixel[] GetRect(int x, int y, int w, int h)
        {
            try
            {
                var tmp = new List<Pixel>();
                for (var i = 0; i < w; i++)
                for (var j = 0; j < h; j++)
                    tmp.Add(GetPixel(x + i, y + j));

                return tmp.ToArray();
            }
            catch (Exception ex)
            {
                throw new SpriteException(this, "Sprite::GetRect", ex);
            }
        }

        /// <summary>
        ///     Returns a small information on what size and exactly how many pixels are in this Sprite
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("H: {0}, W: {1}, Pixels: {2}", Height, Width, Pixels.Length);
        }
    }
}