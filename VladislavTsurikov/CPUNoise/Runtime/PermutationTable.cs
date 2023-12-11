using System;

namespace VladislavTsurikov.CPUNoise.Runtime
{
    internal class PermutationTable
    {

        public int Size { get; private set; }

        public int Seed { get; private set; }

        public float Inverse { get; private set; }
        public int Max { get; private set; }

        private int _wrap;

        private int[] _table;

        internal PermutationTable(int size, int max, int seed)
        {
            Size = size;
            _wrap = Size - 1;
            Max = Math.Max(1, max);
            Inverse = 1.0f / Max;
            Build(seed);
        }

        internal void Build(int seed)
        {
            if (Seed == seed && _table != null) return;

            Seed = seed;
            _table = new int[Size];

            Random rnd = new Random(Seed);

            for(int i = 0; i < Size; i++)
            {
                _table[i] = rnd.Next();
            }
        }

        internal int this[int i]
        {
            get
            {
                return _table[i & _wrap] & Max;
            }
        }

        internal int this[int i, int j]
        {
            get
            {
                return _table[(j + _table[i & _wrap]) & _wrap] & Max;
            }
        }

        internal int this[int i, int j, int k]
        {
            get
            {
                return _table[(k + _table[(j + _table[i & _wrap]) & _wrap]) & _wrap] & Max;
            }
        }

    }
}
