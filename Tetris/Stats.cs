namespace TetrisLib
{
    public sealed class Stats
    {
        private uint _combo = 0;
        private uint _score = 0;
        private uint _level = 1;
        private uint _lines = 0;

        private int _gravity;

        public uint Combo => _combo;
        public uint Score => _score;
        public uint Level => _level;
        public uint Lines => _lines;

        public int Gravity => _gravity;

        public Stats()
        {
            _gravity = _gravityCheatSheet;
        }

        private int _gravityCheatSheet => Level switch
        {
            1 => 800,
            2 => 717,
            3 => 550,
            4 => 467,
            5 => 383,
            6 => 300,
            7 => 216,
            8 => 133,
            9 => 100,
            >= 10 and <= 12 => 83,
            >= 13 and <= 15 => 67,
            >= 16 and <= 18 => 50,
            >= 19 and <= 28 => 33,
            >= 29 => 17,
        };

    public void ClearLines(int amount)
        {
            if(amount == 0)
            {
                _combo = 0;
                return;
            }

            _score += (uint)((_level) * (amount switch
            {
                1 => 100,
                2 => 300,
                3 => 500,
                4 => 800,
            }));

            _lines += (uint)amount;
            if(_lines >= 10 * _level)
            {
                _level++;
                _gravity = _gravityCheatSheet;
            }

            CalculateCombo();
            _combo++;
        }

        public void CalculateCombo()
        {
            _score += (uint)(50 * _combo * _level);
        }
    }
}
