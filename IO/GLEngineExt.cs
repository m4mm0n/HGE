using System;
using OpenTK.Graphics.OpenGL;

namespace HGE.IO
{
    public static class GLEngineExt
    {
        public static int CreateVertexShaderProgram(string vertexShader)
        {
            var vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderID, vertexShader);
            GL.CompileShader(vertexShaderID);

            var vertexInfo = GL.GetShaderInfoLog(vertexShaderID);
            if (vertexInfo != "") Console.WriteLine(vertexInfo);

            var programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);

            var programInfo = GL.GetProgramInfoLog(programID);
            if (programInfo != "") Console.WriteLine(programInfo);

            GL.DetachShader(programID, vertexShaderID);

            GL.DeleteShader(vertexShaderID);

            return programID;
        }

        public static int CreateFragmentShaderProgram(string fragmentShader)
        {
            var fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(fragmentShaderID, fragmentShader);
            GL.CompileShader(fragmentShaderID);

            var fragmentInfo = GL.GetShaderInfoLog(fragmentShaderID);
            if (fragmentInfo != "") Console.WriteLine(fragmentInfo);

            var programID = GL.CreateProgram();
            GL.AttachShader(programID, fragmentShaderID);
            //GL.LinkProgram(programID);

            var programInfo = GL.GetProgramInfoLog(programID);
            if (programInfo != "") Console.WriteLine(programInfo);

            GL.DetachShader(programID, fragmentShaderID);

            GL.DeleteShader(fragmentShaderID);

            return programID;
        }

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
    }
}