#define WIN

namespace TetrisLib.Console
{
    public static class ConsoleExtensions
    {
        static ConsoleExtensions()
        {
/*#if OS_LINUX         
            LinuxConsoleBingings.Init();
#endif*/
        }

        public static void Write(string message)
        {
#if OS_WINDOWS
            WinConsoleBindings.Write(message);
/*#elif OS_LINUX
            LinuxConsoleBingings.Write(message);*/
#else
            System.Console.Write(message);
#endif
        }

        public static void SetCursorPosition(int x, int y)
        {
#if OS_WINDOWS
            WinConsoleBindings.SetCursorPosition(x, y);
/*#elif OS_LINUX
            LinuxConsoleBingings.SetCursorPosition(x, y);*/
#else
            System.Console.SetCursorPosition(x, y);
#endif
        }

        public static void SetColors(ConsoleColor foreground, ConsoleColor backgound)
        {
#if OS_WINDOWS
            WinConsoleBindings.SetColor(foreground, backgound);
/*#elif OS_LINUX
            LinuxConsoleBingings.SetColor(foreground, backgound);*/
#else
            System.Console.ForegroundColor = foreground;
            System.Console.BackgroundColor = backgound;
#endif
        }

        public static void Dispose()
        {
/*#if OS_LINUX         
            LinuxConsoleBingings.Dispose();
#endif*/
        }

        public static void ReadInput(Action<ConsoleKey> action)
        {
            var task = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(10);
                    if (System.Console.KeyAvailable)
                    {
                        var key = System.Console.ReadKey(true).Key;
                        action(key);
                    }
                }
            });
            task.Wait();
        }
    }
}
