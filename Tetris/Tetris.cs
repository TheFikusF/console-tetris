using System.Text;
using Terminal.Gui;

namespace TetrisLib
{
    public class Tetris
    {
        public const int HEIGHT = 24;
        public const int WIDTH = 10;

        private Stats _stats;
        private bool _gameIsPlaying;
        private bool _stashed = false;

        private readonly Position _startingPosition = new Position(4, 0);

        private int[,] _field;
        private StringBuilder _builder;

        private Tetramino _currentPiece;
        private Tetramino _nextPiece;
        private Tetramino _stashedPiece;
        
        private Action<string> _drawAction;
        private Action<string> _drawNext;
        private Action<string> _drawStash;
        private Action<Stats> _drawScore;

        public Tetris(Action<string> drawBoard, Action<string> drawNext, Action<string> drawStash, Action<Stats> drawScore)
        {
            _builder = new StringBuilder();

            _drawAction = drawBoard;
            _drawNext = drawNext;
            _drawStash = drawStash;
            _drawScore = drawScore;
        }

        public void StartGame()
        {
            _gameIsPlaying = true;

            _stats = new Stats();
            _currentPiece = null;
            _stashedPiece = null;
            _nextPiece = null;

            _field = new int[WIDTH, HEIGHT];
            NewPiece();
            DrawBoard();
            Update();
        }

        private async Task Update()
        {
            await Task.Delay(_stats.Gravity);
            
            if (_gameIsPlaying)
            {
                MovePiecePosition(new Position(0, 1));
                DrawBoard();
            
                await Update();
            }
        }

        private void DrawBoard()
        {
            _builder.Clear();

            bool WillPaint(int x, int y)
            {
                var local = ToLocal(new Position(x, y));
                bool inShape = local.X < _currentPiece.Size && local.Y < _currentPiece.Size && local.X >= 0 && local.Y >= 0;
                return _field[x, y] == 1 || (inShape && _currentPiece.Shape[local.X, local.Y] == 1);
            }

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    _builder.Append(WillPaint(x, y) ? "██" : "  ");
                }
                _builder.AppendLine();
            }

            _drawAction?.Invoke(_builder.ToString());
            _drawNext?.Invoke(GetPreview(_nextPiece));
            _drawStash?.Invoke(GetPreview(_stashedPiece));
            _drawScore?.Invoke(_stats);
        }

        public void HandleInput(Key key)
        {
            switch (key)
            {
                case Key.d or Key.D or Key.CursorRight:
                    MovePiecePosition(new Position(1, 0));
                    break;
                case Key.a or Key.A or Key.CursorLeft:
                    MovePiecePosition(new Position(-1, 0));
                    break;
                case Key.w or Key.W or Key.CursorUp:
                    Rotate();
                    break;
                case Key.s or Key.S or Key.CursorDown:
                    MovePiecePosition(new Position(0, 1));
                    break;
                case Key.ShiftMask:
                    Stash();
                    break;
                case Key.Space:
                    int i = 0;
                    while (i < 22 && _currentPiece.Position.Y != 0)
                    {
                        MovePiecePosition(new Position(0, 1));
                        i++;
                    }
                    break;
                case Key.R or Key.r:
                    _gameIsPlaying = false;
                    break;
            }

            DrawBoard();
        }

        private void Stash()
        {
            if(_stashed)
            {
                return;
            }

            var piece = _currentPiece;
            _currentPiece = null;

            if (_stashedPiece == null)
            {
                NewPiece();
            }
            else
            {
                _currentPiece = _stashedPiece;
                _currentPiece.Position = _startingPosition;
            }

            _stashedPiece = piece;

            _stashed = true;

            DrawBoard();
        }

        private void Rotate()
        {
            bool failed = false;
            _currentPiece.Iterate((x, y) =>
            {
                var global = ToGlobal(x, y);

                if (global.X >= WIDTH || global.X < 0 || global.Y >= HEIGHT)
                {
                    failed = true;
                    return;
                }

                if (_currentPiece.RotatedShape[x, y] == 1 && _field[global.X, global.Y] == 1)
                {
                    failed = true;
                }
            });

            if(!failed)
            {
                _currentPiece.Rotate();
            }
        }

        private void NewPiece()
        {
            if (_currentPiece != null)
            {
                StopPiece();
            }

            TryClearLines();

            _stashed = false;

            _currentPiece = _nextPiece ?? new Tetramino()
            {
                Position = _startingPosition
            };

            _nextPiece = new Tetramino()
            {
                Position = _startingPosition
            };
        }

        private void TryClearLines()
        {
            int deletedLines = 0;
            for (int y = 0; y < HEIGHT; y++)
            {
                bool filled = true;
                for (int x = 0; x < WIDTH; x++)
                {
                    if(_field[x, y] == 0)
                    {
                        filled = false;
                    }
                }

                if(filled)
                {
                    deletedLines++;
                    DeleteLine(y);
                }
            }

            _stats.ClearLines(deletedLines);
        }

        private void StopPiece()
        {
            _currentPiece.Iterate((x, y) =>
            {
                var global = ToGlobal(x, y);
                if (global.X >= 0 && global.X < WIDTH && global.Y < HEIGHT && _currentPiece.Shape[x, y] == 1)
                {
                    _field[global.X, global.Y] = 1;
                }
            });

            if(_currentPiece.Position.Y < 4)
            {
                _gameIsPlaying = false;
            }
        }

        private void DeleteLine(int i)
        {
            for(int y = i; y > 1; y--)
            {
                for(int x = 0; x < WIDTH; x++)
                {
                    _field[x, y] = _field[x, y - 1];
                }
            }
        }

        private void MovePiecePosition(Position move)
        {
            for (int y = 0; y < _currentPiece.Size; y++)
            {
                for (int x = 0; x < _currentPiece.Size; x++)
                {
                    var global = ToGlobal(x + move.X, y + move.Y);

                    if (_currentPiece.Shape[x, y] == 0)
                    {
                        continue;
                    }

                    if (global.X >= 0 && global.X < WIDTH && global.Y < HEIGHT && (_field[global.X, global.Y] != 0))
                    {
                        if (move.Y != 0)
                        {
                            NewPiece();
                        }

                        return;
                    }

                    if ((global.X < 0 || global.X >= WIDTH))
                    {
                        return;
                    }

                    if (global.Y >= HEIGHT)
                    {
                        NewPiece();
                        return;
                    }
                }
            }

            _currentPiece.Position += move;
        }

        private Position ToLocal(Position position) => ToLocal(position.X, position.Y);
        private Position ToLocal(int x, int y) => new Position(x + 1 - _currentPiece.Position.X, y - _currentPiece.Position.Y);

        private Position ToGlobal(Position position) => ToGlobal(position.X, position.Y);
        private Position ToGlobal(int x, int y) => new Position(x - 1 + _currentPiece.Position.X, y + _currentPiece.Position.Y);

        private string GetPreview(Tetramino tetramino)
        {
            if(tetramino == null)
            {
                return new string(' ', 12);
            }

            _builder.Clear();

            bool WillPaint(int x, int y)
            {
                var local = new Position(x + 1 - 2, y - 1);
                bool inShape = local.X < tetramino.Size && local.Y < tetramino.Size && local.X >= 0 && local.Y >= 0;
                return (inShape && tetramino.Shape[local.X, local.Y] == 1);
            }

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    _builder.Append(WillPaint(x, y) ? "██" : "  ");
                }
                _builder.AppendLine();
            }

            return _builder.ToString();
        }
    }
}
