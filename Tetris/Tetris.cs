﻿using System.Text;

namespace TetrisLib
{
    public sealed class Tetris
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
        private Tetramino _holdPiece;

        public event Action OnDraw;
        public int[,] Field => _field;
        public int[,] CurrentPiece => _currentPiece.Shape;
        public int[,] NextPiece => _nextPiece?.Shape ?? new int[2, 2];
        public int[,] HoldPiece => _holdPiece?.Shape ?? new int[2, 2];
        public uint Score => _stats.Score;
        public uint Level => _stats.Level;
        public uint Combo => _stats.Combo;
        public uint Lines => _stats.Lines;

        public Tetris()
        {
            _builder = new StringBuilder();
        }

        public void StartGame()
        {
            _gameIsPlaying = true;

            _stats = new Stats();
            _currentPiece = null;
            _holdPiece = null;
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
/*            _builder.Clear();

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
*/
            OnDraw?.Invoke();
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D or ConsoleKey.RightArrow:
                    MovePiecePosition(new Position(1, 0));
                    break;
                case ConsoleKey.A or ConsoleKey.LeftArrow:
                    MovePiecePosition(new Position(-1, 0));
                    break;
                case ConsoleKey.W or ConsoleKey.UpArrow:
                    Rotate();
                    break;
                case ConsoleKey.S or ConsoleKey.DownArrow:
                    MovePiecePosition(new Position(0, 1));
                    break;
                case ConsoleKey.C:
                    Stash();
                    break;
                case ConsoleKey.Spacebar:
                    int i = 0;
                    while (i < 22 && _currentPiece.Position.Y != 0)
                    {
                        MovePiecePosition(new Position(0, 1));
                        i++;
                    }
                    break;
                case ConsoleKey.R:
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

            if (_holdPiece == null)
            {
                NewPiece();
            }
            else
            {
                _currentPiece = _holdPiece;
                _currentPiece.Position = _startingPosition;
            }

            _holdPiece = piece;

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

                if (_currentPiece.RotatedShape[x, y] > 0 && _field[global.X, global.Y] > 0)
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
                if (global.X >= 0 && global.X < WIDTH && global.Y < HEIGHT && _currentPiece.Shape[x, y] > 0)
                {
                    _field[global.X, global.Y] = _currentPiece.Shape[x, y];
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

        public Position ToLocal(Position position) => ToLocal(position.X, position.Y);
        public Position ToLocal(int x, int y) => new Position(x + 1 - _currentPiece.Position.X, y - _currentPiece.Position.Y);

        public Position ToGlobal(Position position) => ToGlobal(position.X, position.Y);
        public Position ToGlobal(int x, int y) => new Position(x - 1 + _currentPiece.Position.X, y + _currentPiece.Position.Y);
    }
}
