namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
        Dictionary<uint, bool[]> DifferentZeroPositions { get; }
        Dictionary<uint, int> SequencecesWithKOnes { get; }

        short Inputs { get; }

        Comparator[] Comparators { get; set; }

        HashSet<short> Outputs { get; }

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
