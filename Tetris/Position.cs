namespace TetrisLib
{
    public struct Position
    {
        public static readonly Position Left = new Position(-1, 0);
        public static readonly Position Right = new Position(1, 0);
        public static readonly Position Zero = new Position(0, 0);
        public static readonly Position Up = new Position(0, -1);
        public static readonly Position Down = new Position(0, 1);

        public readonly int X;
        public readonly int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Position operator -(Position a, Position b) => new Position(a.X - b.X, a.Y - b.Y);
        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);
    }
}
