using TetrisLib;

static Tetris Start()
{
    var tetris = new Tetris();
    var drawer = new TetrisDrawer(tetris);

    tetris.StartGame();
    return tetris;
}

var tetris = Start();

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        tetris.HandleInput(key);
        if(key == ConsoleKey.R)
        {
            tetris = Start();
        }
    }
}