using System.Runtime.InteropServices;

namespace TetrisLib.Console
{
    public static class WinConsoleBindings
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InputRecord
        {
            public EventType EventType;
            public KeyInputRecord KeyInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyInputRecord
        {
            public short ScanCode;
            public char Char;
            public uint ControlKeyState;
        }

        public enum EventType
        {
            KeyDown = 1,
            KeyUp = 2,
            Mouse = 3,
            WindowBufferSizeChange = 4,
            Menu = 5,
            Focus = 6
        }

        private static readonly Dictionary<short, ConsoleKey> _scanCodeMap = new Dictionary<short, ConsoleKey>
        {
            { 0x1C, ConsoleKey.Enter }, // Enter
//            { 0x2A, ConsoleKey.Shif }, // Left Shift
            { 0x39, ConsoleKey.Spacebar }, // Space
            { 0x41, ConsoleKey.A }, // A
            { 0x43, ConsoleKey.C }, // C
            { 0x50, ConsoleKey.D }, // D
            { 0x51, ConsoleKey.UpArrow }, // Up Arrow
            { 0x52, ConsoleKey.DownArrow }, // Down Arrow
            { 0x53, ConsoleKey.LeftArrow }, // Left Arrow
            { 0x54, ConsoleKey.RightArrow }, // Right Arrow
            { 0x57, ConsoleKey.S }, // S
            { 0x5A, ConsoleKey.W }, // W
        };

        public const int STANDART_OUTPUT_HANDLE = -11;
        private static readonly IntPtr _stdHandler;

        static WinConsoleBindings()
        {
            _stdHandler = GetStdHandle(STANDART_OUTPUT_HANDLE);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);

        public static void Write(string message)
        {
            uint charsWritten;
            if (!WriteConsole(_stdHandler, message, (uint)message.Length, out charsWritten, IntPtr.Zero))
            {
                throw new Exception("Failed to write to console");
            }
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        public static void SetCursorPosition(int x, int y)
        {
            if (!SetConsoleCursorPosition(_stdHandler, new COORD { X = (short)x, Y = (short)y }))
            {
                throw new Exception("Failed to set cursor position");
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, uint wAttributes);

        public static void SetColor(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            uint color = 0;

            if (foregroundColor != ConsoleColor.Black)
            {
                color |= (uint)foregroundColor;
            }

            if (backgroundColor != ConsoleColor.Black)
            {
                color |= ((uint)backgroundColor << 4);
            }

            if (!SetConsoleTextAttribute(_stdHandler, color))
            {
                throw new Exception("Failed to set console color");
            }
        }

        [DllImport("kernel32.dll")]
        static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] InputRecord[] inputBuffer, uint nLength, out uint nRead);

        [DllImport("kernel32.dll")]
        static extern bool PeekConsoleInput(IntPtr hConsoleInput, [Out] InputRecord[] inputBuffer, uint nLength, out uint nRead);

        public static async Task<ConsoleKeyInfo> ReadKeyAsync()
        {
            while (true)
            {
                InputRecord[] inputBuffer = new InputRecord[1];
                uint nRead;

                if (!PeekConsoleInput(GetStdHandle(-10), inputBuffer, 1, out nRead))
                {
                    throw new Exception("Failed to peek console input");
                }

                if (nRead == 0)
                {
                    await Task.Delay(10); // Wait briefly if no input is available
                    continue;
                }

                if (inputBuffer[0].EventType == EventType.KeyDown)
                {
                    ReadConsoleInput(GetStdHandle(-10), inputBuffer, 1, out nRead); // Remove the key event from the input buffer
                    return new ConsoleKeyInfo(inputBuffer[0].KeyInput.Char, _scanCodeMap[inputBuffer[0].KeyInput.ScanCode], false, false, false);
                }
            }
        }
    }
}
