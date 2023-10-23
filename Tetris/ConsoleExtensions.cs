namespace TetrisLib.Console
{
    public static class ConsoleExtensions
    {
        private static readonly bool _isWindows;

        static ConsoleExtensions()
        {
            _isWindows = System.OperatingSystem.IsWindows();
        }

        public static void Write(string message)
        {
            if(_isWindows)
            {
                WinConsoleBindings.Write(message);
            }
            else
            {
                System.Console.Write(message);
            }
        }

        public static void SetCursorPosition(int x, int y)
        {
            if (_isWindows)
            {
                WinConsoleBindings.SetCursorPosition(x, y);
            }
            else
            {
                System.Console.SetCursorPosition(x, y);
            }
        }

        public static void SetColors(ConsoleColor foreground, ConsoleColor backgound)
        {
            if (_isWindows)
            {
                WinConsoleBindings.SetColor(foreground, backgound);
            }
            else
            {
                System.Console.ForegroundColor = foreground;
                System.Console.BackgroundColor = backgound;
            }
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
