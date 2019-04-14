using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace HGE.Graphics
{
    using ImageScalar = Byte;
    using Scalar = Single;

    public class BitmapBlur : IDisposable
    {
        private readonly Bitmap myBitmap;
        private InternalBitmapBlur ibb;

        public BitmapBlur(Bitmap bmpToBlur)
        {
            myBitmap = (Bitmap) bmpToBlur.Clone();
        }

        public Bitmap GetBlurred
        {
            get
            {
                ibb = new InternalBitmapBlur(2f);
                ibb.ApplyToBitmap(myBitmap, Rectangle.Empty, PixelFormat.Format32bppArgb, 4);

                return myBitmap;
            }
        }

        public void Dispose()
        {
            myBitmap?.Dispose();
            ibb = null;
        }

        internal class InternalBitmapBlur
        {
            #region Private Constants

            /// <summary>
            ///     Number of channels in image to process.
            /// </summary>
            private const int ciChannels = 4;

            #endregion

            #region Initialisation

            /// <summary>
            ///     Creates a new blur class with a certain radius.
            /// </summary>
            /// <param name="radius">The radius for the blur.</param>
            public InternalBitmapBlur(float radius)
            {
                Radius = radius;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the readius of the blur.
            /// </summary>
            public float Radius
            {
                get => mfRadius;
                set
                {
                    if (mfRadius > 255.0f)
                        throw new ArgumentOutOfRangeException("Radius", "Must be in range of 0.0 - 255f");

                    if (mfRadius != value)
                    {
                        miRadius = (int) Math.Ceiling(value);
                        mfRadius = value;
                        mbTablesInvalid = true;
                    }
                }
            }

            #endregion

            #region Protected Locals

            /// <summary>
            ///     True if the tables are invalid and need repopulating.
            /// </summary>
            protected bool mbTablesInvalid = true;

            /// <summary>
            ///     The radius of the blur convolution, size = 1 + (radius * 2).
            /// </summary>
            protected int miRadius;

            /// <summary>
            ///     The radius of the blur operation.
            /// </summary>
            protected float mfRadius;

            /// <summary>
            ///     The last calculated kernel.
            /// </summary>
            protected float[] msKernel;

            /// <summary>
            ///     The last calculated kernel multiply table.
            /// </summary>
            protected float[,] msMulTable;

            #endregion

            #region Protected Virtual Methods

            /// <summary>
            ///     Converts from a float value to a scalar value.
            /// </summary>
            /// <param name="value">The value to convert.</param>
            /// <returns>A scalar value.</returns>
            protected virtual float FloatToScalar(float value)
            {
                return value;
            }

            /// <summary>
            ///     Recalculates the kernel and kernel multiply tables if needed.
            /// </summary>
            protected virtual void ReCalculateTables()
            {
                // If the tables are invalid
                if (mbTablesInvalid)
                {
                    // Cache values
                    var liSize = miRadius * 2 + 1;
                    msKernel = new float[liSize];
                    msMulTable = new float[liSize, 256];
                    var lfKernelSum = 0.0f;
                    float lfRadius;

                    for (var liKernelIndex = 1; liKernelIndex <= miRadius; liKernelIndex++)
                    {
                        // If its the outside radial value then use the floating
                        // point part if radius isnt a whole value
                        if (liKernelIndex == miRadius && miRadius != mfRadius)
                            lfRadius = mfRadius - (float) Math.Truncate(mfRadius);
                        else
                            // Otherwise just use the whole part
                            lfRadius = 1.0f + (miRadius - liKernelIndex);

                        // Calculate left and right values for kernel
                        var liLeft = miRadius - liKernelIndex;
                        var liRight = miRadius + liKernelIndex;

                        // Set the values
                        msKernel[liRight] = msKernel[liLeft] = FloatToScalar(lfRadius);

                        // Add to the kernel sum
                        lfKernelSum += FloatToScalar(lfRadius) * 2.0f;

                        // Set the multiply table values
                        for (var liValue = 0; liValue < 256; liValue++)
                            msMulTable[liRight, liValue] =
                                msMulTable[liLeft, liValue] = FloatToScalar(lfRadius) * liValue;
                    }

                    // Set the center kernel value to 1 + radius
                    lfRadius = 1.0f + mfRadius;
                    msKernel[miRadius] = FloatToScalar(lfRadius);

                    // Set the multiply table values for the center kernel
                    for (var j = 0; j < 256; j++)
                        msMulTable[miRadius, j] = FloatToScalar(lfRadius) * j;

                    // Validate the tables
                    mbTablesInvalid = false;
                }
            }

            #endregion

            #region Protected Methods

            /// <summary>
            ///     Proceses horizontal scanlines for bluring.
            /// </summary>
            /// <param name="width">The width of the source and dest parameters.</param>
            /// <param name="height">The height of the source and dest parameters.</param>
            /// <param name="source">The source data, one element per channel, width then height.</param>
            /// <param name="dest">The destination data, one element per channel, width then height.</param>
            /// <param name="startLine">The starting line to process.</param>
            /// <param name="countLines">The number of lines to process.</param>
            protected void ProcessHorizontal(
                int width, int height,
                byte[] source, byte[] dest,
                int startLine, int countLines)
            {
                // Sums for channels and kernel
                float lsBSum, lsGSum, lsRSum, lsASum, lsKSum;

                // Channel width in elements
                var liChannelWidth = width * ciChannels;

                // Kernel channel width - 1
                var liKernelChannelsMinus1 = (msKernel.Length - 1) * ciChannels;

#if !Unsafe
                unsafe
                {
                    // Lock the source and dest for GC
                    fixed (byte* lpsSource = source, lpsDest = dest)
                    {
                        // Lock the multiply tables and kernel tables
                        fixed (float* lpsMultable = msMulTable, lpsKernel = msKernel)
                        {
                            // Pointer to multiply table
                            float* lpsMulTableBase;

                            // Destination pointer
                            var lpsDestPoint = &lpsDest[0];

                            // Source kernel point pointer
                            var lpsKernelPoint = &lpsSource[-(miRadius * ciChannels)];

                            // Kernel point start pointer
                            var lpsKernelPointStart = lpsSource;

                            // Kernel point end pointer
                            var lpsKernelPointEnd = lpsSource + liChannelWidth;

                            // Offset pointers for starting line
                            var liStartPoint = liChannelWidth * startLine;
                            lpsKernelPointStart += liStartPoint;
                            lpsKernelPointEnd += liStartPoint;
                            lpsKernelPoint += liStartPoint;
                            lpsDestPoint += liStartPoint;

                            // Process number of lines requested
                            for (var liY = 0; liY < countLines; liY++)
                            {
                                // Process each pixel in scan line
                                for (var liX = 0; liX < width; liX++)
                                {
                                    // Default sums
                                    lsBSum = lsGSum = lsRSum = lsASum = lsKSum = 0;

                                    // Set the multiply table base
                                    lpsMulTableBase = lpsMultable;

                                    // For each kernel value
                                    for (var liKernelIndex = 0; liKernelIndex < msKernel.Length; liKernelIndex++)
                                    {
                                        // If in range then add to the kernel sums
                                        if (lpsKernelPoint >= lpsKernelPointStart && lpsKernelPoint < lpsKernelPointEnd)
                                        {
                                            lsBSum += lpsMulTableBase[lpsKernelPoint[0]];
                                            lsGSum += lpsMulTableBase[lpsKernelPoint[1]];
                                            lsRSum += lpsMulTableBase[lpsKernelPoint[2]];
                                            lsASum += lpsMulTableBase[lpsKernelPoint[3]];
                                            lsKSum += lpsKernel[liKernelIndex];
                                        }

                                        // Increment to kernel line
                                        lpsMulTableBase += 256;

                                        // Incremenent kernel point
                                        lpsKernelPoint += ciChannels;
                                    }

                                    // Calculate averages for blur
                                    lpsDestPoint[0] = (byte) (lsBSum / lsKSum);
                                    lpsDestPoint[1] = (byte) (lsGSum / lsKSum);
                                    lpsDestPoint[2] = (byte) (lsRSum / lsKSum);
                                    lpsDestPoint[3] = (byte) (lsASum / lsKSum);

                                    // Next destination point
                                    lpsDestPoint += ciChannels;

                                    // Reset kernel point
                                    lpsKernelPoint -= liKernelChannelsMinus1;
                                }

                                // Increment start and end kernel points
                                lpsKernelPointStart += liChannelWidth;
                                lpsKernelPointEnd += liChannelWidth;
                            }
                        }
                    }
                }
#else
// Index for destination point
            int liDestPoint = 0;

            // Index for kernel point
            int liKernelPoint = -(miRadius * ciChannels);

            // Start index for kernel row
            int liKernelPointStart = 0;

            // End index for kernel row
            int liKernelPointEnd = liChannelWidth;

            // Offset pointers for starting line
            int liStartPoint = liChannelWidth * startLine;
            liKernelPointStart += liStartPoint;
            liKernelPointEnd += liStartPoint;
            liKernelPoint += liStartPoint;
            liDestPoint += liStartPoint;

            // Process number of lines requested
            for (int liY = 0; liY < countLines; liY++)
            {
                // Process each pixel in scan line
                for (int liX = 0; liX < width; liX++)
                {
                    // Default sums
                    lsBSum = lsGSum = lsRSum = lsASum = lsKSum = 0;

                    // For each kernel value
                    for (int liKernelIndex = 0; liKernelIndex < msKernel.Length; liKernelIndex++)
                    {
                        // If in range then add to the kernel sums
                        if (liKernelPoint >= liKernelPointStart && liKernelPoint < liKernelPointEnd)
                        {
                            lsBSum += msMulTable[liKernelIndex, (int)source[liKernelPoint]];
                            lsGSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 1]];
                            lsRSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 2]];
                            lsASum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 3]];
                            lsKSum += msKernel[liKernelIndex];
                        }

                        // Incremenent kernel point
                        liKernelPoint += ciChannels;
                    }

                    // Calculate averages for blur
                    dest[liDestPoint] = (ImageScalar)(lsBSum / lsKSum);
                    dest[liDestPoint + 1] = (ImageScalar)(lsGSum / lsKSum);
                    dest[liDestPoint + 2] = (ImageScalar)(lsRSum / lsKSum);
                    dest[liDestPoint + 3] = (ImageScalar)(lsASum / lsKSum);

                    // Next destination point
                    liDestPoint += ciChannels;

                    // Reset kernel point
                    liKernelPoint -= liKernelChannelsMinus1;
                }

                // Increment start and end kernel points
                liKernelPointStart += liChannelWidth;
                liKernelPointEnd += liChannelWidth;
            }
#endif
            }

            /// <summary>
            ///     Proceses vertical lines for bluring.
            /// </summary>
            /// <param name="width">The width of the source and dest parameters.</param>
            /// <param name="height">The height of the source and dest parameters.</param>
            /// <param name="destData">An IntPtr to the memory for the destination bitmap.</param>
            /// <param name="destDataStride">The scan line stride size in bytes for the dest data parameter.</param>
            /// <param name="destDataLineOffset">
            ///     The number of bytes from the end of a line to the start of the next for the dest data
            ///     parameter.
            /// </param>
            /// <param name="source">The source data, one element per channel, width then height.</param>
            /// <param name="startLine">The starting horizontal scan line to process.</param>
            /// <param name="countLines">The number of horizontal scan lines to process.</param>
            protected void ProcessVertical(
                int width, int height,
                IntPtr destData, int destDataStride, int destDataLineOffset,
                byte[] source,
                int startLine, int countLines)
            {
                // Sums for channels and kernel
                float lsBSum, lsGSum, lsRSum, lsASum, lsKSum;

                // Channel width in elements
                var liChannelWidth = width * ciChannels;

                // Element count for next kernel column
                var liKernelNextCol = ciChannels - msKernel.Length * liChannelWidth;

#if !Unsafe
                unsafe
                {
                    // Lock the source for GC
                    fixed (byte* lpsSource = source)
                    {
                        // Lock the multiply tables and kernel tables
                        fixed (float* lpsMultable = msMulTable, lpsKernel = msKernel)
                        {
                            // Multiply kernel table base
                            float* lpsMulTableBase;

                            // Destination data pointer
                            var lpbDest = (byte*) destData;

                            // Kernel point start index 
                            // (never changes due to columns, kernel point
                            // range is set to entire source range)
                            var lpsKernelPointStart = lpsSource;

                            // Kernel end pointer
                            var lpsKernelPointEnd = lpsSource + height * liChannelWidth;

                            // Current kernel point
                            var lpsKernelPoint = lpsSource - miRadius * liChannelWidth;

                            // Offset pointers for starting line
                            lpsKernelPoint += liChannelWidth * startLine;
                            lpbDest += destDataStride * startLine;

                            // Process number of lines requested
                            for (var liY = 0; liY < countLines; liY++)
                            {
                                // Process each pixel
                                for (var liX = 0; liX < width; liX++)
                                {
                                    // Reset sums
                                    lsBSum = lsGSum = lsRSum = lsASum = lsKSum = 0;

                                    // Reset multiply table base
                                    lpsMulTableBase = lpsMultable;

                                    // For each kernel value
                                    for (var liKernelIndex = 0; liKernelIndex < msKernel.Length; liKernelIndex++)
                                    {
                                        // If in range of current column then add sums
                                        if (lpsKernelPoint >= lpsKernelPointStart && lpsKernelPoint < lpsKernelPointEnd)
                                        {
                                            lsBSum += lpsMulTableBase[lpsKernelPoint[0]];
                                            lsGSum += lpsMulTableBase[lpsKernelPoint[1]];
                                            lsRSum += lpsMulTableBase[lpsKernelPoint[2]];
                                            lsASum += lpsMulTableBase[lpsKernelPoint[3]];
                                            lsKSum += lpsKernel[liKernelIndex];
                                        }

                                        // Next multiply table base
                                        lpsMulTableBase += 256;

                                        // Next kernel point column
                                        lpsKernelPoint += liChannelWidth;
                                    }

                                    // Average sums for blur
                                    lpbDest[0] = (byte) (lsBSum / lsKSum);
                                    lpbDest[1] = (byte) (lsGSum / lsKSum);
                                    lpbDest[2] = (byte) (lsRSum / lsKSum);
                                    lpbDest[3] = (byte) (lsASum / lsKSum);

                                    // Next destination point
                                    lpbDest += 4;

                                    // Next kernel column
                                    lpsKernelPoint += liKernelNextCol;
                                }

                                // Next destination line
                                lpbDest += destDataLineOffset;
                            }
                        }
                    }
                }
#else
// Destination index
            int liDest = 0;

            // Kernel point start index 
            // (never changes due to columns, kernel point
            // range is set to entire source range)
            const int liKernelPointStart = 0;

            // Kernel point end index
            int liKernelPointEnd = (height * liChannelWidth);

            // Current kernel source point
            int liKernelPoint = -(miRadius * liChannelWidth);

            // Offset indexes for starting line
            liKernelPoint += liChannelWidth * startLine;
            liDest += destDataStride * startLine;

            // Process number of lines requested
            for (int liY = 0; liY < countLines; liY++)
            {
                // Process each pixel
                for (int liX = 0; liX < width; liX++)
                {
                    // Reset sums
                    lsBSum = lsGSum = lsRSum = lsASum = lsKSum = 0;

                    // For each kernel value
                    for (int liKernelIndex = 0; liKernelIndex < msKernel.Length; liKernelIndex++)
                    {
                        // If in range of current column then add sums
                        if (liKernelPoint >= liKernelPointStart && liKernelPoint < liKernelPointEnd)
                        {
                            lsBSum += msMulTable[liKernelIndex, (int)source[liKernelPoint]];
                            lsGSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 1]];
                            lsRSum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 2]];
                            lsASum += msMulTable[liKernelIndex, (int)source[liKernelPoint + 3]];
                            lsKSum += msKernel[liKernelIndex];
                        }

                        // Next kernel point column
                        liKernelPoint += liChannelWidth;
                    }

                    // Average sums for blur
                    Marshal.WriteByte(destData, liDest, (byte)(lsBSum / lsKSum));
                    Marshal.WriteByte(destData, liDest + 1, (byte)(lsGSum / lsKSum));
                    Marshal.WriteByte(destData, liDest + 2, (byte)(lsRSum / lsKSum));
                    Marshal.WriteByte(destData, liDest + 3, (byte)(lsASum / lsKSum));

                    // Next destination point
                    liDest += 4;

                    // Next kernel column
                    liKernelPoint += liKernelNextCol;
                }

                // Next destination line
                liDest += destDataLineOffset;
            }
#endif
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///     Applys sharpening to a Bitmap.
            /// </summary>
            /// <param name="bitmap">The Bitmap to apply the sharpening to.</param>
            /// <param name="rectOfInterest">The rectangle to apply the Effect or Rectangle.Empty for entire bitmap.</param>
            /// <param name="pixelFormat">The pixel format to use (must be a 32-bit pixel format).</param>
            /// <param name="threads">The maximum number of threads in addition to the current thread to use.</param>
            public void ApplyToBitmap(Bitmap bitmap, Rectangle rectOfInterest, PixelFormat pixelFormat, int threads)
            {
                if (rectOfInterest.IsEmpty) rectOfInterest = new Rectangle(new Point(), bitmap.Size);

                if (Radius > 0)
                {
                    var lbdData = bitmap.LockBits(rectOfInterest, ImageLockMode.ReadWrite, pixelFormat);

                    try
                    {
                        Process(lbdData, lbdData, threads);
                    }
                    finally
                    {
                        bitmap.UnlockBits(lbdData);
                    }
                }
            }

            /// <summary>
            ///     Processes the blur effect.
            /// </summary>
            /// <param name="src">The source bitmap data.</param>
            /// <param name="dest">The destination bitmap data.</param>
            /// <param name="threads">The maximum number of threads in addition to the current thread to use.</param>
            public void Process(BitmapData src, BitmapData dest, int threads)
            {
                // Check the tables to ensure they are updated
                ReCalculateTables();

                // Create the temporary data holders
                var liPixelCount = src.Width * src.Height;
                var lsHor = new byte[liPixelCount * ciChannels];
                var lsBoth = new byte[liPixelCount * ciChannels];

                // Calculate the number of bytes from the end of one line to the start of the next for src
                var liSrcDataLineOffset = src.Stride - src.Width * 4;

                // Calculate the number of bytes from the end of one line to the start of the next for src
                var liDestDataLineOffset = dest.Stride - dest.Width * 4;

#if !Unsafe
                // Copy the data from the source to the array
                unsafe
                {
                    // Fix the horizontal array
                    fixed (byte* lpsHor = lsHor)
                    {
                        // Get the start src pixel
                        var lpbPixel = (byte*) src.Scan0;

                        // Get the start dest pixel
                        var lpsDest = lpsHor;

                        // For each line
                        for (var liY = 0; liY < src.Height; liY++)
                        {
                            // For each pixel
                            for (var liX = 0; liX < src.Width; liX++)
                            {
                                // Copy the data from bytes to image scalars
                                lpsDest[0] = lpbPixel[0];
                                lpsDest[1] = lpbPixel[1];
                                lpsDest[2] = lpbPixel[2];
                                lpsDest[3] = lpbPixel[3];

                                // Incrmement the dest pixel
                                lpsDest += ciChannels;

                                // Increment the source pixel
                                lpbPixel += 4;
                            }

                            // Increment the src line
                            lpbPixel += liSrcDataLineOffset;
                        }
                    }
                }
#else
// Dest index
            int liIndex = 0;

            // Source index
            int liPixelIndex = 0;

            // For each line
            for (int liY = 0; liY < src.Height; liY++)
            {
                // For each pixel
                for (int liX = 0; liX < src.Width; liX++)
                {
                    // Copy the data from bytes to image scalars
                    lsHor[liIndex] = Marshal.ReadByte(src.Scan0, liPixelIndex);
                    lsHor[liIndex + 1] = Marshal.ReadByte(src.Scan0, liPixelIndex + 1);
                    lsHor[liIndex + 2] = Marshal.ReadByte(src.Scan0, liPixelIndex + 2);
                    lsHor[liIndex + 3] = Marshal.ReadByte(src.Scan0, liPixelIndex + 3);

                    // Next destination pixel
                    liIndex += ciChannels;

                    // Next source pixel
                    liPixelIndex += 4;
                }

                // Next source line
                liPixelIndex += liSrcDataLineOffset;
            }
#endif
                // Calculate number of liens
                var liTotalLines = src.Height;

                // Reduce threads to maximum of number of lines - 1
                if (liTotalLines < threads + 1) threads = liTotalLines - 1;

                // Calculate lines per thread
                var liLinesPerThread = liTotalLines / (threads + 1);

                // Thread arrays
                var ltThreads = new Thread[threads];
                var lareStartedEvents = new AutoResetEvent[threads];

                // Two modes, Horizontal and Vertical, must be processed seperately for blur effect
                for (var liMode = 0; liMode <= 1; liMode++)
                {
                    // Line offset
                    var liLineOffset = 0;

                    // For each thread
                    for (var liCounter = 0; liCounter < threads; liCounter++)
                    {
                        // Cache line offset for thread
                        var liCurrLineOffset = liLineOffset;

                        // Create the started event
                        lareStartedEvents[liCounter] = new AutoResetEvent(false);

                        // If mode 0 then 
                        if (liMode == 0)
                            // Execute horizontal processing
                            ltThreads[liCounter] = new Thread(
                                args =>
                                {
                                    ((AutoResetEvent) ((object[]) args)[0]).Set();
                                    ProcessHorizontal(
                                        src.Width, src.Height,
                                        lsHor, lsBoth,
                                        liCurrLineOffset, liLinesPerThread);
                                }
                            );
                        else
                            // Otherwise execute vertical processing
                            ltThreads[liCounter] = new Thread(
                                args =>
                                {
                                    ((AutoResetEvent) ((object[]) args)[0]).Set();
                                    ProcessVertical(
                                        src.Width, src.Height,
                                        dest.Scan0, dest.Stride, liDestDataLineOffset,
                                        lsBoth, liCurrLineOffset, liLinesPerThread);
                                }
                            );

                        // Start the thread
                        ltThreads[liCounter].Start(new object[] {lareStartedEvents[liCounter]});

                        // Incremenent the line offset
                        liLineOffset += liLinesPerThread;
                    }

                    var liLines = liTotalLines - liLinesPerThread * threads;

                    // If mode 0 then 
                    if (liMode == 0)
                        // Execute horizontal processing
                        ProcessHorizontal(
                            src.Width, src.Height,
                            lsHor, lsBoth,
                            liLineOffset, liLines);
                    else
                        // Otherwise execute vertical processing
                        ProcessVertical(
                            src.Width, src.Height,
                            dest.Scan0, dest.Stride, liDestDataLineOffset,
                            lsBoth, liLineOffset, liLines);

                    // Ensure each thread has started
                    foreach (var lareEvent in lareStartedEvents)
                        lareEvent.WaitOne();

                    // Ensure each thread has finished
                    foreach (var ltThread in ltThreads)
                        ltThread.Join();
                }
            }

            #endregion
        }
    }
}