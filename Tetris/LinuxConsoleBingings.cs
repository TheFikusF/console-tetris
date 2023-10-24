using System.Runtime.InteropServices;

#if OS_LINUX
namespace TetrisLib.Console
{
    public static class LinuxConsoleBingings
    {
        [DllImport("libncurses.so.5")]
        private static extern int initscr();

        [DllImport("libncurses.so.5")]
        private static extern int endwin();

        [DllImport("libncurses.so.5")]
        private static extern int refresh();

        [DllImport("libncurses.so.5")]
        private static extern int printw(string format, params object[] args);

        [DllImport("libncurses.so.5")]
        private static extern int mvprintw(int y, int x, string format, params object[] args);

        [DllImport("libncurses.so.5")]
        private static extern int move(int y, int x);

        [DllImport("libncurses.so.5")]
        private static extern int attron(int attrs);

        [DllImport("libncurses.so.5")]
        private static extern int attroff(int attrs);

        [DllImport("libncurses.so.5")]
        private static extern int init_pair(short pair, short foreground, short background);

        private static int COLOR_PAIR(int pair)
        {
            return (pair << 8);
        }

        private static int A_COLOR(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return 0;
                case ConsoleColor.DarkRed:
                    return 1;
                case ConsoleColor.DarkGreen:
                    return 2;
                case ConsoleColor.DarkYellow:
                    return 3;
                case ConsoleColor.DarkBlue:
                    return 4;
                case ConsoleColor.DarkMagenta:
                    return 5;
                case ConsoleColor.DarkCyan:
                    return 6;
                case ConsoleColor.Gray:
                    return 7;
                case ConsoleColor.DarkGray:
                    return 8;
                case ConsoleColor.Red:
                    return 9;
                case ConsoleColor.Green:
                    return 10;
                case ConsoleColor.Yellow:
                    return 11;
                case ConsoleColor.Blue:
                    return 12;
                case ConsoleColor.Magenta:
                    return 13;
                case ConsoleColor.Cyan:
                    return 14;
                case ConsoleColor.White:
                    return 15;
                default:
                    return 0;
            }
        }

        private static void InitColorPairs()
        {
            for (int foreground = 0; foreground <= 15; foreground++)
            {
                init_pair((short)(foreground * 16), (short)foreground, 0);

                init_pair((short)(foreground * 16 + 1), (short)foreground, 15);
            }
        }

        public static void Init()
        {
            if (initscr() == -1)
            {
                throw new Exception("Failed to initialize console");
            }

            InitColorPairs();
        }

        public static void Dispose()
        {
            endwin();
        }

        public static void Write(string message)
        {
            printw(message);
            refresh();
        }

        public static void SetCursorPosition(int x, int y)
        {
            move(y, x);
        }

        public static void SetColor(ConsoleColor foreground, ConsoleColor background)
        {
            int pair = 0;

            pair = background switch
            {
                ConsoleColor.Black => (int)foreground * 16,
                ConsoleColor.White => (int)foreground * 16 + 1,
                _ => throw new ArgumentException("Unsupported background color"),
            };

            int attrs = COLOR_PAIR(pair);
            attron(attrs);
        }

        public static void ResetColor()
        {
            attroff(COLOR_PAIR(0));
        }
    }
}
#endif