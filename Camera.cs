using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace GameEngine
{
    class Camera
    {
        private Vector3 position = new Vector3();
        private Vector3 rotation = new Vector3();
        private float fov = 60;
        private float cameraSensitivity = 1 / 800f;
        private Matrix4? projectionMatrix = null;
        private Vector2 rotationConstraints = new Vector2(-MathF.PI / 2, MathF.PI / 2);

        public Camera(Vector3 startingPosition, Vector3 startingRotation, float fov = 60, float cameraSensitivity = 1 / 800f)
        {
            position = startingPosition;
            rotation = startingRotation;
            this.fov = fov;
            this.cameraSensitivity = cameraSensitivity;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov / 180f * MathF.PI,
                (float)Program.GetWindow()!.Size.X / Program.GetWindow()!.Size.Y, 0.1f, 1000f);
        }

        public Camera()
        {
            Program.GetWindow().KeyDown += OnKeyDown;
            Program.GetWindow().KeyUp += OnKeyUp;
            Program.GetWindow().RenderFrame += Update;
            Program.GetWindow().MouseMove += OnMouseMove;
            position = new Vector3();
            rotation = new Vector3();
            fov = 60;
            cameraSensitivity = 1 / 800f;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fov / 180f * MathF.PI,
                (float)Program.GetWindow()!.Size.X / Program.GetWindow()!.Size.Y, 0.1f, 1000f);
        }

        private bool w, a, s, d, e, q;
        private void Update(FrameEventArgs e)
        {
            float moveSpeed = 5;
            if (w)
                Move(Forward() * (float)e.Time * moveSpeed);
            if (a)
                Move(Right() * (float)e.Time * moveSpeed);
            if (s)
                Move(Backward() * (float)e.Time * moveSpeed);
            if (d)
                Move(Left() * (float)e.Time * moveSpeed);
            if (this.e)
                Move(Vector3.UnitY * (float)e.Time * moveSpeed);
            if (q)
                Move(-Vector3.UnitY * (float)e.Time * moveSpeed);
        }

        private void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch(e.Key)
            {
                case Keys.W:
                    w = true;
                    break;
                case Keys.A:
                    a = true;
                    break;
                case Keys.S:
                    s = true;
                    break;
                case Keys.D:
                    d = true;
                    break;
                case Keys.E:
                    this.e = true;
                    break;
                case Keys.Q:
                    q = true;
                    break;
            }
        }
        private void OnKeyUp(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.W:
                    w = false;
                    break;
                case Keys.A:
                    a = false;
                    break;
                case Keys.S:
                    s = false;
                    break;
                case Keys.D:
                    d = false;
                    break;
                case Keys.E:
                    this.e = false;
                    break;
                case Keys.Q:
                    q = false;
                    break;
            }
        }

        public Matrix4? GetProjectionMatrix()
            => projectionMatrix;
        public void SetProjectionMatrix(Matrix4 projectionMatrix)
            => this.projectionMatrix = projectionMatrix;

        public void Move(Vector3 movement)
        {
            position += movement;
        }

        public void Rotate(Vector3 rotation)
        {
            this.rotation += rotation * cameraSensitivity;
            this.rotation.X = Math.Clamp(this.rotation.X, rotationConstraints.X, rotationConstraints.Y);
        }

        public Vector3 Position() => position;
        public Vector3 Forward() => (Vector4.UnitZ * CameraMatrix().Inverted()).Xyz;
        public Vector3 Right() => Vector3.Cross(Forward(), Vector3.UnitY);
        public Vector3 Left() => -Right();
        public Vector3 Backward() => -Forward();

        public float GetFOV() => angleToRad(fov);
        public float angleToRad(float angle) => angle * MathF.PI / 180;
        public float radToAngle(float rad) => rad / MathF.PI * 180;

        public Matrix4 CameraMatrix()
            => Matrix4.CreateTranslation(position) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));

        public Matrix4 CameraRotationMatrix()
            => Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
        public Matrix4 CameraTranslationMatrix()
            => Matrix4.CreateTranslation(position);

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            Rotate(new Vector3(-e.DeltaY, -e.DeltaX, 0f));
        }
    }
}
