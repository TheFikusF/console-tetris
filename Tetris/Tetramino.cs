namespace TetrisLib
{
    public enum Shape
    {
        Square,
        Line,
        L,
        J,
        T,
        Z,
        S,
    }

    public sealed class Tetramino
    {
        private Shape _shape;

        private int[,] _matrix;
        private int[,] _rotatedMatrix;
        private Position _position;

        private readonly int _size;
        private readonly int[][,] _shapes = new int[7][,]
        {
            new int[2, 2] {
                { 1, 1},
                { 1, 1},
            },
            new int[4, 4] {
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 },
            },
            new int[3, 3] {
                { 0, 0, 3 },
                { 3, 3, 3 },
                { 0, 0, 0 },
            },
            new int[3, 3] {
                { 4, 0, 0 },
                { 4, 4, 4 },
                { 0, 0, 0 },
            },
            new int[3, 3] {
                { 0, 5, 0 },
                { 5, 5, 0 },
                { 0, 5, 0 },
            },
            new int[3, 3] {
                { 0, 6, 0 },
                { 6, 6, 0 },
                { 6, 0, 0 },
            },
            new int[3, 3] {
                { 7, 0, 0 },
                { 7, 7, 0 },
                { 0, 7, 0 },
            },
        };

        public Position Position { get => _position; set => _position = value; }
        public int[,] Shape => _matrix;
        public int[,] RotatedShape => _matrix;
        public int Size => _size;

        public static readonly ConsoleColor[] Colors = new ConsoleColor[8]
        {
            ConsoleColor.Black,
            ConsoleColor.Yellow,
            ConsoleColor.Cyan,
            ConsoleColor.Magenta,
            ConsoleColor.Blue,
            ConsoleColor.DarkMagenta,
            ConsoleColor.Red,
            ConsoleColor.Green,
        };

        public Tetramino()
        {
            _shape = (Shape)Extensions.Random.Next(0, 7);
            _matrix = _shapes[(int)_shape];
            _size = _matrix.GetLength(0);

            _rotatedMatrix = Extensions.RotateMatrix(_matrix, Size);
        }

        public void Iterate(Action<int, int> action)
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    action(x, y);
                }
            }
        }

        public void Rotate()
        {
            _matrix = _rotatedMatrix;
            _rotatedMatrix = Extensions.RotateMatrix(_rotatedMatrix, Size);
        }
    }
}
