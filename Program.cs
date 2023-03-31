using System;

namespace GameEngine
{
    class Program
    {
        private static Window window = new Window();
        public static Window GetWindow() => window;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            window = new Window();
            window.Run();
        }
    }
}
