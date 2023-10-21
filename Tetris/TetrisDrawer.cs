using System.Drawing;

namespace TetrisLib
{
    public sealed class TetrisDrawer
    {
        private bool _drawing;
        private bool _pendingDraw;
        private readonly Tetris _tetris;

        private readonly Position _size = new Position(Tetris.WIDTH, Tetris.HEIGHT);
        private readonly Position _previewSize = new Position(6, 4);

        private readonly Position _fieldOffset = new Position(1, 1);
        private readonly Position _scoreOffset = new Position(24, 1);
        private readonly Position _nextOffset = new Position(24, 4);
        private readonly Position _holdOffset = new Position(24, 11);
        private readonly Position _statsOffset = new Position(24, 18);

        public TetrisDrawer(Tetris tetris)
        {
            Console.CursorVisible = false;
            Console.Title = "T E T R I S";

            _tetris = tetris;
            _tetris.OnDraw += Draw;
        }

        private int GetBigBoardPixel(int x, int y)
        {
            var local = _tetris.ToLocal(x, y);
            bool inShape = local.X < _tetris.CurrentPiece.GetLength(0) && local.Y < _tetris.CurrentPiece.GetLength(0) && local.X >= 0 && local.Y >= 0;

            return (inShape && _tetris.CurrentPiece[local.X, local.Y] > 0)
                ? _tetris.CurrentPiece[local.X, local.Y]
                : _tetris.Field[x, y];
        }

        private int GetNextBoardPixel(int x, int y) => GetSmallBoardPixel(_tetris.NextPiece, x, y);
        private int GetHoldBoardPixel(int x, int y) => GetSmallBoardPixel(_tetris.HoldPiece, x, y);

        private int GetSmallBoardPixel(int[,] shape, int x, int y)
        {
            var local = new Position(x + 1 - 2, y - 1);
            bool inShape = local.X < shape.GetLength(0) && local.Y < shape.GetLength(0) && local.X >= 0 && local.Y >= 0;

            return (inShape && shape[local.X, local.Y] > 0)
                ? shape[local.X, local.Y]
                : 0;
        }

        private void Draw()
        {
            if(_drawing)
            {
                _pendingDraw = true;
                return;
            }

            _pendingDraw = false;
            _drawing = true;

            DrawBlock(GetBigBoardPixel, _size, _fieldOffset, "╡ TETRIS" + ((_tetris.Combo > 1) ? $" COMBO X{_tetris.Combo - 1} ╞" : " ╞"));
            DrawBlock(GetNextBoardPixel, _previewSize, _nextOffset, "╡ NEXT ╞");
            DrawBlock(GetHoldBoardPixel, _previewSize, _holdOffset, "╡ HOLD ╞");
            Console.SetCursorPosition(_scoreOffset.X, _scoreOffset.Y);
            Console.Write("┌┤ SCORE ├───┐");
            Console.SetCursorPosition(_scoreOffset.X, _scoreOffset.Y+1);
            Console.Write($"└┤{_tetris.Score.ToString().PadLeft(10)}├┘");

            Console.SetCursorPosition(_statsOffset.X, _statsOffset.Y);
            Console.Write("┌┤ STATS ├───┐");
            Console.SetCursorPosition(_statsOffset.X, _statsOffset.Y + 1);
            Console.Write($"├Lines {_tetris.Lines.ToString().PadLeft(6)}┤");
            Console.SetCursorPosition(_statsOffset.X, _statsOffset.Y + 2);
            Console.Write($"└Level {_tetris.Level.ToString().PadLeft(6)}┘");

            _drawing = false;

            if(_pendingDraw)
            {
                Draw();
            }
        }

        private void DrawBlock(Func<int, int, int> getPixel, Position size, Position position, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(position.X, position.Y);
            Console.Write($"╔{title}{new string('═', size.X * 2 - title.Length)}╗");
            for (int y = 0; y < size.Y; y++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(position.X, y + position.Y + 1);
                Console.Write("║");
                for (int x = 0; x < size.X; x++)
                {
                    int pixel = getPixel(x, y);
                    Console.ForegroundColor = Tetramino.Colors[pixel];
                    Console.Write("██");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("║");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(position.X, size.Y + position.Y + 1);
            Console.Write($"╚{new string('═', size.X * 2)}╝");
        }
    }
}
