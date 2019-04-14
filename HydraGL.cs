using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace HGE
{
    internal interface IGLControl
    {
        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000022 RID: 34
        bool IsIdle { get; }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000023 RID: 35
        IWindowInfo WindowInfo { get; }

        // Token: 0x06000021 RID: 33
        IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags);
    }

    internal class WinGLControl : IGLControl
    {
        // Token: 0x04000014 RID: 20
        private readonly GraphicsMode mode;

        // Token: 0x04000012 RID: 18
        private MSG msg = default(MSG);

        // Token: 0x04000013 RID: 19

        // Token: 0x06000041 RID: 65 RVA: 0x000027CB File Offset: 0x000017CB
        public WinGLControl(GraphicsMode mode, Control control)
        {
            this.mode = mode;
            WindowInfo = Utilities.CreateWindowsWindowInfo(control.Handle);
        }

        // Token: 0x06000042 RID: 66 RVA: 0x000027F7 File Offset: 0x000017F7
        public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
        {
            return new GraphicsContext(mode, WindowInfo, major, minor, flags);
        }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000043 RID: 67 RVA: 0x0000280D File Offset: 0x0000180D
        public bool IsIdle => !PeekMessage(ref msg, IntPtr.Zero, 0, 0, 0);

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x06000044 RID: 68 RVA: 0x00002825 File Offset: 0x00001825
        public IWindowInfo WindowInfo { get; }

        // Token: 0x06000040 RID: 64
        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        private static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax,
            int flags);

        // Token: 0x0200000B RID: 11
        private struct MSG
        {
            // Token: 0x06000045 RID: 69 RVA: 0x00002830 File Offset: 0x00001830
            public override string ToString()
            {
                return string.Format("msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}",
                    (int) Message, Message, HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
            }

            // Token: 0x04000015 RID: 21
            public IntPtr HWnd;

            // Token: 0x04000016 RID: 22
            public uint Message;

            // Token: 0x04000017 RID: 23
            public IntPtr WParam;

            // Token: 0x04000018 RID: 24
            public IntPtr LParam;

            // Token: 0x04000019 RID: 25
            public uint Time;

            // Token: 0x0400001A RID: 26
            public POINT Point;
        }

        // Token: 0x0200000C RID: 12
        private struct POINT
        {
            // Token: 0x06000046 RID: 70 RVA: 0x000028B2 File Offset: 0x000018B2
            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            // Token: 0x06000047 RID: 71 RVA: 0x000028C2 File Offset: 0x000018C2
            public Point ToPoint()
            {
                return new Point(X, Y);
            }

            // Token: 0x06000048 RID: 72 RVA: 0x000028D8 File Offset: 0x000018D8
            public override string ToString()
            {
                return string.Concat("Point {", X.ToString(), ", ", Y.ToString(), ")");
            }

            // Token: 0x0400001B RID: 27
            public readonly int X;

            // Token: 0x0400001C RID: 28
            public readonly int Y;
        }
    }

    internal class GLControlFactory
    {
        // Token: 0x06000001 RID: 1 RVA: 0x000020D0 File Offset: 0x000010D0
        public IGLControl CreateGLControl(GraphicsMode mode, Control control)
        {
            if (mode == null) throw new ArgumentNullException("mode");
            if (control == null) throw new ArgumentNullException("control");
            //if (Configuration.RunningOnSdl2)
            //{
            //    return new Sdl2GLControl(mode, control);
            //}
            if (Configuration.RunningOnWindows) return new WinGLControl(mode, control);
            //if (Configuration.RunningOnMacOS)
            //{
            //    return new CarbonGLControl(mode, control);
            //}
            //if (Configuration.RunningOnX11)
            //{
            //    return new X11GLControl(mode, control);
            //}
            throw new PlatformNotSupportedException();
        }
    }

    internal class DummyGLControl : IGLControl
    {
        // Token: 0x06000024 RID: 36 RVA: 0x000026C4 File Offset: 0x000016C4
        public IGraphicsContext CreateContext(int major, int minor, GraphicsContextFlags flags)
        {
            return new DummyContext();
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000025 RID: 37 RVA: 0x000026CB File Offset: 0x000016CB
        public bool IsIdle => false;

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000026 RID: 38 RVA: 0x000026CE File Offset: 0x000016CE
        public IWindowInfo WindowInfo => Utilities.CreateDummyWindowInfo();

        // Token: 0x02000007 RID: 7
        private class DummyContext : IGraphicsContext, IDisposable, IGraphicsContextInternal
        {
            // Token: 0x0400000B RID: 11
            private static int instance_count;

            // Token: 0x0400000C RID: 12

            // Token: 0x0400000D RID: 13
            private IWindowInfo current_window;

            // Token: 0x0400000E RID: 14

            // Token: 0x0400000F RID: 15

            // Token: 0x1700000F RID: 15
            // (get) Token: 0x0600002C RID: 44 RVA: 0x000026FE File Offset: 0x000016FE
            // (set) Token: 0x0600002D RID: 45 RVA: 0x0000270C File Offset: 0x0000170C
            public bool VSync
            {
                get => SwapInterval != 0;
                set => SwapInterval = value ? 1 : 0;
            }

            // Token: 0x06000028 RID: 40 RVA: 0x000026DD File Offset: 0x000016DD
            public void SwapBuffers()
            {
            }

            // Token: 0x06000029 RID: 41 RVA: 0x000026DF File Offset: 0x000016DF
            public void MakeCurrent(IWindowInfo window)
            {
                current_window = window;
            }

            // Token: 0x1700000D RID: 13
            // (get) Token: 0x0600002A RID: 42 RVA: 0x000026E8 File Offset: 0x000016E8
            public bool IsCurrent => current_window != null;

            // Token: 0x1700000E RID: 14
            // (get) Token: 0x0600002B RID: 43 RVA: 0x000026F6 File Offset: 0x000016F6
            public bool IsDisposed { get; private set; }

            // Token: 0x17000010 RID: 16
            // (get) Token: 0x0600002E RID: 46 RVA: 0x0000271B File Offset: 0x0000171B
            // (set) Token: 0x0600002F RID: 47 RVA: 0x00002723 File Offset: 0x00001723
            public int SwapInterval { get; set; }

            // Token: 0x06000030 RID: 48 RVA: 0x0000272C File Offset: 0x0000172C
            public void Update(IWindowInfo window)
            {
            }

            // Token: 0x17000011 RID: 17
            // (get) Token: 0x06000031 RID: 49 RVA: 0x0000272E File Offset: 0x0000172E
            public GraphicsMode GraphicsMode => GraphicsMode.Default;

            // Token: 0x17000012 RID: 18
            // (get) Token: 0x06000032 RID: 50 RVA: 0x00002735 File Offset: 0x00001735
            // (set) Token: 0x06000033 RID: 51 RVA: 0x00002738 File Offset: 0x00001738
            public bool ErrorChecking
            {
                get => false;
                set { }
            }

            // Token: 0x06000034 RID: 52 RVA: 0x0000273A File Offset: 0x0000173A
            public void LoadAll()
            {
            }

            // Token: 0x06000035 RID: 53 RVA: 0x0000273C File Offset: 0x0000173C
            public void Dispose()
            {
                IsDisposed = true;
            }

            // Token: 0x17000013 RID: 19
            // (get) Token: 0x06000036 RID: 54 RVA: 0x00002745 File Offset: 0x00001745
            public ContextHandle Context { get; } =
                new ContextHandle(new IntPtr(Interlocked.Increment(ref instance_count)));

            // Token: 0x06000037 RID: 55 RVA: 0x0000274D File Offset: 0x0000174D
            public IntPtr GetAddress(IntPtr function)
            {
                return IntPtr.Zero;
            }

            // Token: 0x06000038 RID: 56 RVA: 0x00002754 File Offset: 0x00001754
            public IntPtr GetAddress(string function)
            {
                return IntPtr.Zero;
            }

            // Token: 0x17000014 RID: 20
            // (get) Token: 0x06000039 RID: 57 RVA: 0x0000275B File Offset: 0x0000175B
            public IGraphicsContext Implementation => this;
        }
    }

    public partial class HydraGL : UserControl
    {
        public delegate void DelayUpdate();

        private const int WS_EX_LAYERED = 0x00080000;
        private const int CS_VREDRAW = 0x1;
        private const int CS_HREDRAW = 0x2;
        private const int CS_OWNDC = 0x20;
        private const int CS_PARENTDC = 0x0080;
        private readonly bool design_mode;
        private readonly GraphicsContextFlags flags;
        private readonly GraphicsMode format;
        private readonly int major;
        private readonly int minor;
        private IGraphicsContext context;


        private IGLControl implementation;
        private bool? initial_vsync_value;
        private bool resize_event_suppressed;

        public HydraGL() : this(GraphicsMode.Default)
        {
        }

        public HydraGL(GraphicsMode mode) : this(mode, 1, 0, GraphicsContextFlags.Default)
        {
        }

        public HydraGL(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
        {
            if (mode == null) throw new ArgumentNullException("mode");
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative
            });
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = true;
            format = mode;
            this.major = major;
            this.minor = minor;
            this.flags = flags;
            design_mode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            InitializeComponent();
        }

        [Description("The aspect ratio of the client area of this GLControl.")]
        public float AspectRatio
        {
            get
            {
                ValidateState();
                return ClientSize.Width / (float) ClientSize.Height;
            }
        }

        [Browsable(false)]
        public IGraphicsContext Context
        {
            get
            {
                ValidateState();
                return context;
            }
            private set => context = value;
        }

        public GraphicsMode GraphicsMode
        {
            get
            {
                ValidateState();
                return Context.GraphicsMode;
            }
            //set
            //{
            //    var tmp = this.Context;

            //}
        }

        [Browsable(false)]
        public bool IsIdle
        {
            get
            {
                ValidateState();
                return Implementation.IsIdle;
            }
        }

        [Description("Indicates whether GLControl updates are synced to the monitor's refresh rate.")]
        public bool VSync
        {
            get
            {
                if (!IsHandleCreated) return initial_vsync_value == null || initial_vsync_value.Value;
                ValidateState();
                return Context.SwapInterval != 0;
            }
            set
            {
                if (!IsHandleCreated)
                {
                    initial_vsync_value = value;
                    return;
                }

                ValidateState();
                Context.SwapInterval = value ? 1 : 0;
            }
        }

        public IWindowInfo WindowInfo => implementation.WindowInfo;

        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;
                if (Configuration.RunningOnWindows) createParams.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                return createParams;
            }
        }

        private IGLControl Implementation
        {
            get
            {
                ValidateState();
                return implementation;
            }
        }

        public Bitmap GrabScreenshot()
        {
            ValidateState();
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            var bitmapData = bitmap.LockBits(ClientRectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, ClientSize.Width, ClientSize.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            return bitmap;
        }

        public void MakeCurrent()
        {
            ValidateState();
            Context.MakeCurrent(Implementation.WindowInfo);
        }

        public void PerformContextUpdate()
        {
            if (context != null) context.Update(Implementation.WindowInfo);
        }

        public void SwapBuffers()
        {
            ValidateState();
            Context.SwapBuffers();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (context != null) context.Dispose();
            if (implementation != null) implementation.WindowInfo.Dispose();
            if (design_mode)
                implementation = new DummyGLControl();
            else
                implementation = new GLControlFactory().CreateGLControl(format, this);
            context = implementation.CreateContext(major, minor, flags);
            MakeCurrent();
            if (!design_mode) ((IGraphicsContextInternal) Context).LoadAll();
            if (initial_vsync_value != null)
            {
                Context.SwapInterval = initial_vsync_value.Value ? 1 : 0;
                initial_vsync_value = null;
            }

            base.OnHandleCreated(e);
            if (resize_event_suppressed)
            {
                OnResize(EventArgs.Empty);
                resize_event_suppressed = false;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }

            if (implementation != null)
            {
                implementation.WindowInfo.Dispose();
                implementation = null;
            }

            base.OnHandleDestroyed(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ValidateState();
            if (design_mode) e.Graphics.Clear(Color.Black);
            base.OnPaint(e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (context != null) context.Update(Implementation.WindowInfo);
            base.OnParentChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (!IsHandleCreated)
            {
                resize_event_suppressed = true;
                return;
            }

            if (Configuration.RunningOnMacOS)
            {
                var method = new DelayUpdate(PerformContextUpdate);
                BeginInvoke(method);
            }
            else if (context != null)
            {
                context.Update(Implementation.WindowInfo);
            }

            base.OnResize(e);
        }

        private void ValidateContext(string message)
        {
            var isCurrent = Context.IsCurrent;
        }

        private void ValidateState()
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().Name);
            if (!IsHandleCreated) CreateControl();
            if (implementation == null || context == null || context.IsDisposed) RecreateHandle();
        }
    }
}