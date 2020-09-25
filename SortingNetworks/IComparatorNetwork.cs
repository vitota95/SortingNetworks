namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
        ushort Inputs { get; }

        Comparator[] Comparators { get; set; }

        HashSet<ushort> Outputs { get; }

        Dictionary<uint, int> DifferentZeroPositions { get; }

        Dictionary<uint, int> SequencesWithKOnes { get; }

        bool IsSubsumed(IComparatorNetwork n, IEnumerable<int> [] permutations);

        bool IsRedundant(IComparatorNetwork n);

        bool IsSortingNetwork();

        IComparatorNetwork CloneWithNewComparator(Comparator comparator);
    }

    public struct Comparator
    {
        public ushort X;
        public ushort Y;

        public Comparator(ushort x, ushort y) 
        {
            this.X = x;
            this.Y = y;
        }
    }
}
