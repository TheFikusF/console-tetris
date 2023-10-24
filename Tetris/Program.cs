using TetrisLib;
using TetrisLib.Console;

uint level = 1;
bool ghostBlock = false;

int levelArg = Array.IndexOf(args, "-l");
levelArg = levelArg == -1 ? Array.IndexOf(args, "--level") : levelArg;
if(levelArg + 1 > 0 && levelArg + 1 < args.Length && int.TryParse(args[levelArg + 1], out int result))
{
    level = (uint)result;
}

if (args.Contains("-g") || args.Contains("--ghost-block"))
{
    ghostBlock = true;
}

Tetris Start()
{
    var tetris = new Tetris();
    tetris.DrawGhostBlock = ghostBlock;
    var drawer = new TetrisDrawer(tetris);

    tetris.StartGame(level);
    return tetris;
}

var tetris = Start();

ConsoleExtensions.ReadInput((key) =>
{
    tetris.HandleInput(key);
    if (key == ConsoleKey.R)
    {
        tetris = Start();
    }

    if (key == ConsoleKey.E)
    {
        ghostBlock = !ghostBlock;
    }
});