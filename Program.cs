using System;

namespace GameEngine
{
    class Program
    {
        private static Window window = new Window();
        public static Window GetWindow() => window;
        public const string LOCALPATH = "../../../";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            window = new Window();
            window.Run();
        }
    }
}
