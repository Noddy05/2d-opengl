using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Shapes
{
    class Draw
    {
        private static GUIElement rectangle;
        private static ShaderRect shaderRect;
        private static GUIElement circle;
        private static Window window;

        public static void Rect(float x, float y, float width, float height, Color4 color)
        {
            Matrix4 transformationMatrix = Matrix4.CreateTranslation(new Vector3(x / width, y / height, 0))
                * Matrix4.CreateScale(new Vector3(width / window.Size.X * 2, height / window.Size.Y * 2, 1))
                * Matrix4.CreateTranslation(new Vector3(-1, -1, 0));
            rectangle.Render(transformationMatrix, color);
        }
        public static void Circle(float x, float y, float radius, Color4 color)
        {
            Matrix4 transformationMatrix = Matrix4.CreateTranslation(new Vector3(x / radius, y / radius, 0))
                * Matrix4.CreateScale(new Vector3(radius * 2 / window.Size.X, radius * 2 / window.Size.Y, 1))
                * Matrix4.CreateTranslation(new Vector3(-radius / window.Size.X - 1, -radius / window.Size.Y - 1, 0));
            circle.Render(transformationMatrix, color);
        }
        public static void ShaderRect(float x, float y, float width, float height, int shader)
        {
            Matrix4 transformationMatrix = Matrix4.CreateTranslation(new Vector3(x / width, y / height, 0f))
                * Matrix4.CreateScale(new Vector3(width / window.Size.X * 2f, height / window.Size.Y * 2f, 1f))
                * Matrix4.CreateTranslation(new Vector3(-1f, -1f, 0));
            shaderRect.Render(transformationMatrix, new Color4(1f, 1f, 1f, 1f));
        }

        public static void OnLoad()
        {
            window = Program.GetWindow();

            int rectShader = Shader.GenerateShader(Program.LOCALPATH + @"\Shapes\rect_vert_shader.glsl",
                Program.LOCALPATH + @"\Shapes\rect_frag_shader.glsl");
            rectangle = new GUIElement(rectShader);

            int raymarchShader = Shader.GenerateShader(Program.LOCALPATH + @"\CustomShader\raymarch_vert_shader.glsl",
                Program.LOCALPATH + @"\CustomShader\raymarch_frag_shader.glsl");
            shaderRect = new ShaderRect(raymarchShader);

            int circleShader = Shader.GenerateShader(Program.LOCALPATH + @"\Shapes\circle_vert_shader.glsl",
                Program.LOCALPATH + @"\Shapes\circle_frag_shader.glsl");
            circle = new GUIElement(circleShader);
        }
    }
}
