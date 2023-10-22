using TetrisLib;

uint level = (args.Length == 2 && (args[0] == "-l" || args[0] == "--level") && int.TryParse(args[1], out int result)) ? (uint)result : 1;

Tetris Start()
{
    var tetris = new Tetris();
    var drawer = new TetrisDrawer(tetris);

    tetris.StartGame(level);
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