namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
        short Inputs { get; }

        Comparator[] Comparators { get; set; }

        HashSet<short> Outputs { get; }

        bool[][] DifferentZeroPositions { get; }

        Dictionary<uint, int> SequencesWithKOnes { get; }

        Dictionary<uint, HashSet<short>> OutputsDictionary { get; }

        Dictionary<uint, bool[]>[] W { get; }

        bool IsSubsumedBy(IComparatorNetwork n, IEnumerable<int>[] permutations);

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
