using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// A GUIElement is a rect that can be displayed on the screen
    /// </summary>
    class ShaderRect
    {
        private int vao, vbo, ibo, shader;
        private int uniformTransformationMatrix;
        private int uniformColor;
        private int uniformScaleRatio;
        private int uniformCameraPosition;
        private int uniformRotation;
        private int uniformT;
        private int uniformLookingAtIndex;
        private const int SIZE = 4;
        private int[] uniformPositions = new int[SIZE * SIZE];
        private int[] uniformSizes = new int[SIZE * SIZE];
        private Stopwatch sw;
        private Vector3[] positions;
        private Vector3[] sizes;

        public int GetShader() => shader;

        public ShaderRect(int shader)
        {
            positions = new Vector3[SIZE * SIZE];
            sizes = new Vector3[SIZE * SIZE];
            Random rand = new Random();
            int seed = 10531172;
            rand = new Random(seed);
            Console.WriteLine(seed);
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    positions[x * SIZE + y] = new Vector3(x - (SIZE - 1) / 2f, -rand.Next(2, 4), y + 14);
                    sizes[x * SIZE + y] = new Vector3(0.5f, rand.Next(0, 3) + 0.5f, 0.5f);
                }
            }
            sw = Stopwatch.StartNew();
            this.shader = shader;

            float[] vertices = new float[]
            {
                -1, -1,
                -1, 1,
                1, 1,
                1, -1,
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
            uniformScaleRatio = GL.GetUniformLocation(shader, "scaleRatio");
            uniformT = GL.GetUniformLocation(shader, "t");
            uniformCameraPosition = GL.GetUniformLocation(shader, "cameraPosition");
            uniformRotation = GL.GetUniformLocation(shader, "rotationMatrix");
            uniformLookingAtIndex = GL.GetUniformLocation(shader, "lookingAtIndex");
            for (int i = 0; i < uniformPositions.Length; i++)
            {
                uniformPositions[i] = GL.GetUniformLocation(shader, $"positions[{i}]");
                uniformSizes[i] = GL.GetUniformLocation(shader, $"sizes[{i}]");
            }
        }
        public void Render(Matrix4 transformationMatrix, Color4 color)
        {
            Window window = Program.GetWindow();
            GL.UseProgram(shader);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo!);
            GL.UniformMatrix4(uniformTransformationMatrix, false, ref transformationMatrix);
            GL.Uniform4(uniformColor, color);
            GL.Uniform1(uniformScaleRatio, (float)window.Size.X / window.Size.Y);
            GL.Uniform1(uniformT, sw.ElapsedMilliseconds / 1000f);

            Camera camera = Program.GetWindow().camera;
            Matrix4 rotationMatrix = camera.CameraRotationMatrix().Inverted();
            GL.Uniform3(uniformCameraPosition, camera.Position());
            GL.UniformMatrix4(uniformRotation, false, ref rotationMatrix);
            for (int i = 0; i < uniformPositions.Length; i++)
            {
                GL.Uniform3(uniformPositions[i], positions[i]);
                GL.Uniform3(uniformSizes[i], sizes[i]);
            }
            GL.Uniform1(uniformLookingAtIndex, GetLookingAtIndex(camera.Position(), camera.Forward()));

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public int GetLookingAtIndex(Vector3 cameraPosition, Vector3 cameraRay)
        {
            float distanceTravelled = 0;
            Vector3 rayVec = cameraRay;
            Vector3 pos = cameraPosition;

            while (distanceTravelled < 100)
            {
                float minStepDistance = SDF(pos, out int closest);

                distanceTravelled += minStepDistance;
                if (minStepDistance < 0.01)
                {
                    return closest;
                }
                pos = cameraPosition + rayVec * distanceTravelled;
            }
            return -1;
        }

        private float SDF(Vector3 position, out int closestIndex)
        {
            float minStepDistance = 10000;
            int minIndex = 0;
            for (int i = 0; i < SIZE * SIZE; i++)
            {
                float dist = DistanceToBox(position - positions[i], sizes[i]);
                if (minStepDistance >= dist)
                {
                    minStepDistance = dist;
                    minIndex = i;
                }
            }

            closestIndex = minIndex;
            return minStepDistance;
        }
        private float DistanceToBox(Vector3 fromRay, Vector3 size)
        {
            return new Vector3(MathF.Max(MathF.Abs(fromRay.X) - size.X, 0), 
                MathF.Max(MathF.Abs(fromRay.Y) - size.Y, 0),
                MathF.Max(MathF.Abs(fromRay.Z) - size.Z, 0)).Length;
        }
    }
}
