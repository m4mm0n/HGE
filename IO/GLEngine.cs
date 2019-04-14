using System;
using System.Windows.Forms;
using HGE.Graphics;
using OpenTK.Graphics.OpenGL;

namespace HGE.IO
{
    public abstract class GLEngine : IEngine
    {
        private const string vertexShader =
            @"
            #version 400 
            precision highp float;
            layout(location = 0) in vec2 pos;
            layout(location = 1) in vec2 tex;
            out vec2 texCoord;
            uniform mat4 projection;
            uniform mat4 model;
            uniform mat4 rotation;
            uniform mat4 scale;
            void main() 
            {
                texCoord = tex;
                gl_Position = projection * scale * model * rotation * vec4(pos.x, pos.y, 0, 1.0);
            }
        ";

        private const string fragmentShader =
            @"
            #version 400
            out vec4 FragColor;
            in vec2 texCoord;
            uniform sampler2D tex;
            void main() {
                FragColor = texture(tex, texCoord);
            } 
        ";

        private const string introShader =
            @"
            precision highp float;
            
            uniform vec2 u_resolution;
            uniform float u_time;
            void main()
            {
                vec2 px = gl_FragCoord.xy/u_resolution.xy;
                vec2 center = vec2(0.5,0.5);
                float dist = distance(px, center) / 2.0;
                vec2 up = vec2(0.0, 1.0);
                vec2 centerVec = normalize(px - center);
                float angle = acos(dot(up, vecterVec)) / 3.141592;
                angle *= 4.0;
                if(px.x >= 0.5) {
                    angle = 1.0-angle;
                }
                angle += sin(u_time) * dist * 2.0;
                angle += u_time / 2.0;
                angle = fract(angle);
                float r = step(0.5, angle);
                vec4 color1 = vec4(0.05, 0.6, 0.8, 1.0) * r;
                vec4 color2 = vec4(0.05, 0.8, 1.0, 1.0) * (1.0 - r);
                vec4 backColor = vec4(0.05, 0.6, 0.8, 1.0);
                vec4 rotateColor = color1 + color2;
                gl_FragColor = mix(backColor, rotateColor, dist);
            }
";

        public Sprite background;
        public int BaseShaderID;
        public float Duration;
        public float Elapsed;

        public HydraGL engineControl;
        public Graphics2DGL graphics;

        public bool IsRunning;
        public RendererGL pixelGraphics;
        private DateTime startTime;

        private DateTime t1, t2;

        public override void Initialize(params object[] parameters)
        {
            Log("Initialize...");

            if (parameters.Length >= 2)
            {
                if (parameters[0] is HydraGL)
                    engineControl = (HydraGL) parameters[0];
                else
                    throw new ArgumentException("Must be a HydraGL!", "parameters[0]");

                if (parameters[1] is int)
                    MaxFPS = (int) parameters[1];
                else
                    throw new ArgumentException("Must be an integer!", "parameters[1]");

                if (parameters.Length >= 3)
                    if (parameters[2] is bool)
                        IsVerbose = (bool) parameters[2];
                    else
                        throw new ArgumentException("Must be a boolean!", "parameters[2]");
                else
                    IsVerbose = false;

                Width = engineControl.Width;
                Height = engineControl.Height;
                HalfHeight = Height / 2;
                HalfWidth = Width / 2;

                background = new Sprite(Width, Height);
                graphics = new Graphics2DGL(Width, Height);
                background.Clear(Pixel.BLACK);

                t1 = t2 = DateTime.Now;
                startTime = DateTime.Now;

                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.MultisampleSgis);

                GL.BlendFunc(BlendingFactor.SrcAlpha,
                    BlendingFactor.OneMinusSrcAlpha);

                BaseShaderID = CreateProgram(vertexShader, fragmentShader);
                pixelGraphics = new RendererGL(this, graphics.DrawTarget, BaseShaderID);

                engineControl.Paint += EngineControl_Paint;
                engineControl.Resize += EngineControl_Resize;
                Application.Idle += Application_Idle;

                EngineControl_Resize(engineControl, EventArgs.Empty);

                engineControl.MakeCurrent();
            }
            else
            {
                throw new ArgumentOutOfRangeException("parameters", "Has to be at least 2 parameters!");
            }
        }

        private void InternalRender()
        {
#if VERBOSELOG
            Log("InternalRender");
#endif

            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0, 0, 0, 1);

            Duration = (float) (DateTime.Now - startTime).TotalSeconds;

            t2 = DateTime.Now;
            Elapsed = (float) (t2 - t1).TotalSeconds;
            t1 = t2;

            background.CopyTo(graphics.DrawTarget);
            pixelGraphics.Render(0, 0, 0, 1, -99, true);

            Update(Elapsed);
            Draw(graphics);
            pixelGraphics.Refresh();
            Render();

            Helpers.Sleep(1000 / MaxFPS);

            engineControl.SwapBuffers();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (IsRunning)
                while (engineControl.IsIdle)
                    InternalRender();
        }

        private void EngineControl_Resize(object sender, EventArgs e)
        {
#if VERBOSELOG
            Log("HydraGL_Resize");
#endif
            GL.Viewport(0, 0, engineControl.Width, engineControl.Height);
        }

        private void EngineControl_Paint(object sender, PaintEventArgs e)
        {
#if VERBOSELOG
            Log("HydraGL_Paint");
#endif
            Render();
        }

        public override void Dispose()
        {
            IsRunning = false;
            engineControl.Paint -= EngineControl_Paint;
            engineControl.Resize -= EngineControl_Resize;
            Application.Idle -= Application_Idle;
            GC.SuppressFinalize(this);
        }

        public override void Clear(Pixel color)
        {
            Log("GLEngine -> Clear :: Color {0}", color.ToString());

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(color.red, color.green, color.blue, color.alpha);
        }

        public virtual void Start()
        {
            Log("GLEngine -> Start");
            IsRunning = true;
        }

        public virtual void Stop()
        {
            Log("GLEngine -> Stop");
            IsRunning = false;
        }

        protected virtual void Update(float elapsed)
        {
#if VERBOSELOG
            Log("GLEngine -> Update :: Elapsed {0}", elapsed);
#endif
        }

        protected virtual void Draw(Graphics2DGL graphics)
        {
#if VERBOSELOG
            Log("GLEngine -> Draw");
#endif
        }

        protected virtual void Render()
        {
#if VERBOSELOG
            Log("GLEngine -> Render");
#endif
        }

        #region GL Only Stuff

        internal static int CreateProgram(string vertexShader, string fragmentShader)
        {
            var vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderID, vertexShader);
            GL.CompileShader(vertexShaderID);

            var vertexInfo = GL.GetShaderInfoLog(vertexShaderID);
            if (vertexInfo != "") Console.WriteLine(vertexInfo);

            GL.ShaderSource(fragmentShaderID, fragmentShader);
            GL.CompileShader(fragmentShaderID);

            var fragmentInfo = GL.GetShaderInfoLog(fragmentShaderID);
            if (fragmentInfo != "") Console.WriteLine(fragmentInfo);

            var programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.LinkProgram(programID);

            var programInfo = GL.GetProgramInfoLog(programID);
            if (programInfo != "") Console.WriteLine(programInfo);

            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);

            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);

            return programID;
        }

        #endregion
    }
}