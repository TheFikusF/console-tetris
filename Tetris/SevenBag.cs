namespace TetrisLib
{
    public class SevenBag
    {
        private readonly int[] _tetris;
        private int _nextIndex;

        public SevenBag(int bagSize)
        {
            _tetris = new int[bagSize];
            for (int i = 0; i < _tetris.Length; i++)
            {
                _tetris[i] = i;
            }

            Extensions.Shuffle(_tetris);
        }

        public int Next()
        {
            if (_nextIndex >= _tetris.Length)
            {
                Extensions.Shuffle(_tetris);
                _nextIndex = 0;
            }

            return _tetris[_nextIndex++];
        }
    }
}
