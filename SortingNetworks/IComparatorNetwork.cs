namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
        short Inputs { get; }

        Comparator[] Comparators { get; set; }

        HashSet<short> Outputs { get; }

        Dictionary<uint, int> DifferentZeroPositions { get; }

        Dictionary<uint, int> SequencesWithKOnes { get; }

        int[] ZeroPositions { get; }

        int[] OnePositions { get; }

        bool IsSubsumed(IComparatorNetwork n, IEnumerable<int> [] permutations);

        bool IsRedundant(IComparatorNetwork n);

        bool IsSortingNetwork();

        IComparatorNetwork CloneWithNewComparator(Comparator comparator);
    }

    public struct Comparator
    {
        public short X;
        public short Y;

        public Comparator(short x, short y) 
        {
            this.X = x;
            this.Y = y;
        }
    }
}
