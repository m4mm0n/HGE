using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HGE.Graphics
{
    public class RendererGL
    {
        private const float orthoFactor = 0.0312f;

        private const string lit_shader = @"
#version 330

in vec3 vPosition;
in vec3 vNormal;
in vec2 texcoord;

out vec3 v_norm;
out vec3 v_pos;
out vec2 f_texcoord;

uniform mat4 modelview;
uniform mat4 model;
uniform mat4 view;

void
main()
{
 gl_Position = modelview * vec4(vPosition, 1.0);
 f_texcoord = texcoord;

 mat3 normMatrix = transpose(inverse(mat3(model)));
 v_norm = normMatrix * vNormal;
 v_pos = (model * vec4(vPosition, 1.0)).xyz;
}
";

        private const string lit_frag = @"
#version 330

in vec3 v_norm;
in vec3 v_pos;
in vec2 f_texcoord;
out vec4 outputColor;

uniform sampler2D maintexture;
uniform mat4 view;

uniform vec3 material_ambient;
uniform vec3 material_diffuse;
uniform vec3 material_specular;
uniform float material_specExponent;

uniform vec3 light_position;
uniform vec3 light_color;
uniform float light_ambientIntensity;
uniform float light_diffuseIntensity;

void
main()
{
 vec2 flipped_texcoord = vec2(f_texcoord.x, 1.0 - f_texcoord.y);
 vec3 n = normalize(v_norm);

 // Colors
 vec4 texcolor = texture2D(maintexture, flipped_texcoord.xy);
 vec4 light_ambient = light_ambientIntensity * vec4(light_color, 0.0);
 vec4 light_diffuse = light_diffuseIntensity * vec4(light_color, 0.0);

 // Ambient lighting
 outputColor = texcolor * light_ambient * vec4(material_ambient, 0.0);

 // Diffuse lighting
 vec3 lightvec = normalize(light_position - v_pos);
 float lambertmaterial_diffuse = max(dot(n, lightvec), 0.0);
 outputColor = outputColor + (light_diffuse * texcolor * vec4(material_diffuse, 0.0)) * lambertmaterial_diffuse;

 // Specular lighting
 vec3 reflectionvec = normalize(reflect(-lightvec, v_norm));
 vec3 viewvec = normalize(vec3(inverse(view) * vec4(0,0,0,1)) - v_pos); 
 float material_specularreflection = max(dot(v_norm, lightvec), 0.0) * pow(max(dot(reflectionvec, viewvec), 0.0), material_specExponent);
 outputColor = outputColor + vec4(material_specular * light_color, 0.0) * material_specularreflection;
}
";

        private readonly IEngine engine;

        private readonly int[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private readonly int projLocation,
            modelLocation,
            scaleLocation,
            rotLocation;

        private readonly int VAO, EBO, textureID;

        private readonly float[] vertices =
        {
            // Vertices    // Texture Coords
            1.0f, 1.0f, 1.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 1.0f
        };

        public RendererGL(IEngine engine, Sprite sprite, int shaderID)
        {
            this.engine = engine;

            Shader = shaderID;
            Sprite = sprite;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            var VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4,
                vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4,
                indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);
            GL.EnableVertexAttribArray(1);

            textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                sprite.Width, sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte,
                sprite.Pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);

            modelLocation = GL.GetUniformLocation(shaderID, "model");
            projLocation = GL.GetUniformLocation(shaderID, "projection");
            rotLocation = GL.GetUniformLocation(shaderID, "rotation");
            scaleLocation = GL.GetUniformLocation(shaderID, "scale");
        }

        public Sprite Sprite { get; }
        public int Shader { get; }

        public void Render(float x, float y, float rotation = 0, float scale = 1,
            float layer = -1, bool fillScreen = false)
        {
            GL.UseProgram(Shader);

            float left = 0, right = fillScreen ? 2 : engine.Width * orthoFactor;
            float bottom = 0, top = fillScreen ? 2 : engine.Height * orthoFactor;
            float posX = x * orthoFactor / scale, posY = y * orthoFactor / scale;

            Matrix4.CreateTranslation(posX + 1, posY + 1, layer, out var model);
            Matrix4.CreateOrthographicOffCenter(left, right, top, bottom, 0.1f, 100f, out var projection);
            Matrix4.CreateRotationZ(rotation, out var rot);
            Matrix4.CreateScale(scale, out var sc);

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BindVertexArray(VAO);

            GL.UniformMatrix4(modelLocation, 1, false, ref model.Row0.X);
            GL.UniformMatrix4(projLocation, 1, false, ref projection.Row0.X);
            GL.UniformMatrix4(scaleLocation, 1, false, ref sc.Row0.X);
            GL.UniformMatrix4(rotLocation, 1, false, ref rot.Row0.X);

            GL.DrawElements(PrimitiveType.Triangles, 8, DrawElementsType.UnsignedInt, 0);
        }

        public void Refresh()
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Sprite.Width, Sprite.Height,
                PixelFormat.Rgba, PixelType.UnsignedByte, Sprite.Pixels);
        }
    }
}