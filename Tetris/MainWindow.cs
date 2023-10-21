using System.Text;
using Terminal.Gui;
using TetrisLib;

public class ExampleWindow : Window
{
    private View _stashView;
    private View _preview;
    private View _textView;
    private View _scoreView;
    private View _statsView;

    private Tetris _tetris;

    public ExampleWindow()
    {
        //Title = "Example App (Ctrl+Q to quit)";

        _textView = new View()
        {
            Title = "T E T R I S",
            BorderStyle = LineStyle.Single,
            ColorScheme = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.White, Color.Black)
            },
            CanFocus = false,
            Height = Tetris.HEIGHT + 2,
            Width = Tetris.WIDTH * 2 + 2,
            X = 1,
            Y = 1,
        };

        _preview = new View()
        {
            Title = "Next",
            BorderStyle = LineStyle.Single,
            ColorScheme = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.White, Color.Black)
            },
            CanFocus = false,
            Height = 7,
            Width = 7 * 2,
            X = Tetris.WIDTH * 2 + 5,
            Y = 6,
        };

        _stashView = new View()
        {
            Title = "Hold",
            BorderStyle = LineStyle.Single,
            ColorScheme = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.White, Color.Black)
            },
            CanFocus = false,
            Height = 7,
            Width = 7 * 2,
            X = Tetris.WIDTH * 2 + 5,
            Y = 14,
        };

        _scoreView = new View()
        {
            Title = "Score",
            BorderStyle = LineStyle.Single,
            CanFocus = false,
            Height = 3,
            Width = 7 * 2,
            X = Tetris.WIDTH * 2 + 5,
            Y = 2,
        };

        _statsView = new View()
        {
            BorderStyle = LineStyle.Single,
            CanFocus = false,
            Height = 4,
            Width = 7 * 2,
            X = Tetris.WIDTH * 2 + 5,
            Y = 22,
        };

        Add(_textView, _stashView, _preview, _scoreView, _statsView);

        StartGame();
    }

    private void StartGame()
    {
        _tetris = new Tetris(
            s => _textView.Text = s,
            s => _preview.Text = s,
            s => _stashView.Text = s,
            s =>
            {
                _scoreView.Text = s.Score.ToString().PadLeft(12, '•');
                _statsView.Text = $"Level: {s.Level}\nLines: {s.Lines}";

                _textView.Title = "TETRIS" + ((s.Combo > 1) ? $" COMBO X{s.Combo - 1}" : "");
            });

        _tetris.StartGame();
    }

    public override bool OnKeyDown(KeyEvent keyEvent)
    {
        _tetris.HandleInput(keyEvent.Key);

        if (keyEvent.Key == Key.R || keyEvent.Key == Key.r)
        {
            StartGame();
        }

        return base.OnKeyDown(keyEvent);
    }
}