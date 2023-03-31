using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    class Shader
    {
        public static int GenerateShader(string vertexShaderPath, string fragmentShaderPath)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertexShader, File.ReadAllText(vertexShaderPath));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(fragmentShaderPath));
            GL.CompileShader(fragmentShader);

            int shader = GL.CreateProgram();
            GL.AttachShader(shader, vertexShader);
            GL.AttachShader(shader, fragmentShader);

            GL.LinkProgram(shader);

            GL.DetachShader(shader, vertexShader);
            GL.DetachShader(shader, fragmentShader);

            return shader;
        }
    }
}
