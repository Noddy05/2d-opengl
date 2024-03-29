﻿using GameEngine.Shapes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Threading;

namespace GameEngine
{
    class Window : GameWindow
    {
        const int frameRate = 120;
        public Camera camera;

        public Window()
               : base(GameWindowSettings.Default, new NativeWindowSettings
               {
                   Title = "Fractal Viewer",
                   Size = new Vector2i(1920, 1080),
                   StartVisible = false,
                   APIVersion = new(3, 3)
               })
        {
            CenterWindow();
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            Draw.OnLoad();
            camera = new Camera();
            CursorState = CursorState.Grabbed;

            IsVisible = true;

            base.OnLoad();
        }

        public float timeElapsed = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Thread.Sleep((int)(1000 / frameRate - args.Time));

            Title = $"Game Window | {MathF.Round(1 / (float)args.Time)} fps " +
                   $"| Time elapsed: {WithDecimals(timeElapsed, 1)}s";
            timeElapsed += (float)args.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            Draw.ShaderRect(0, 0, Size.X, Size.Y, -1);
            //Draw.Rect(MousePosition.X, Size.Y - MousePosition.Y, 150, 150, new Color4(1f, 0f, 1f, 1f));
            Draw.Circle(Size.X / 2, Size.Y / 2, 5, new Color4(1.0f, 1.0f, 1.0f, 1.0f));

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        private static string WithDecimals(float a, int decimals)
        {
            if (MathF.Round(a * MathF.Pow(10, decimals)) / MathF.Pow(10, decimals) - MathF.Round(a) == 0)
                return (MathF.Round(a) + ",0").ToString();
            else
                return (MathF.Round(a * MathF.Pow(10, decimals)) / MathF.Pow(10, decimals)).ToString();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
