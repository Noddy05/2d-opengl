using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// A GUIElement is a rect that can be displayed on the screen
    /// </summary>
    class GUIElement
    {
        private int vao, vbo, ibo, uniformTransformationMatrix, uniformColor, shader;

        public void SetShader(int shader) => this.shader = shader;

        public GUIElement(int shader)
        {
            this.shader = shader;

            float[] vertices = new float[]
            {
                0, 0,
                0, 1,
                1, 1,
                1, 0,
            };

            //  1----2
            //  |    |
            //  0----3
            uint[] indices = new uint[]{
                0, 1, 3,
                1, 2, 3
            };

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticCopy);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticCopy);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            uniformTransformationMatrix = GL.GetUniformLocation(shader, "transform");
            uniformColor = GL.GetUniformLocation(shader, "color");
        }

        public void Render(Matrix4 transformationMatrix, Color4 color)
        {
            GL.UseProgram(shader);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo!);
            GL.UniformMatrix4(uniformTransformationMatrix, false, ref transformationMatrix);
            GL.Uniform4(uniformColor, color);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
    }
}
