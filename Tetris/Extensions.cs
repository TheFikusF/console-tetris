namespace TetrisLib
{
    public static class Extensions
    {
        public static readonly SevenBag Random = new SevenBag(7);

        public static int[,] RotateMatrix(int[,] matrix, int n)
        {
            int[,] ret = new int[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = matrix[n - j - 1, i];
                }
            }

            return ret;
        }

        public static void Shuffle<T>(T[] list)
        {
            Random random = new Random();

            for (int i = list.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
