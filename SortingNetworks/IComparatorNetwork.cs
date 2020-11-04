namespace SortingNetworks
{
    using System;
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
#if DEBUG
        static long SubsumeNoCheck1 { get; set; }
        static long SubsumeNoCheck2 { get; set; }
        static long PermutationsNumber { get; set; }
        static long SubsumeNumber { get; set; }
        static long SubsumeSucceed{ get; set; }
#endif
        static ushort Inputs { get; set; }

        Comparator[] Comparators { get; set; }

        HashSet<ushort> Outputs { get; }

        int[] Where0 { get; }

        int[] Where0SetCount { get; }

        int[] SequencesWithOnes { get; }

        int[] Where1 { get; }

        bool IsSubsumed(IComparatorNetwork n, IEnumerable<int> [] permutations);

        bool IsRedundant(IComparatorNetwork n);

        bool IsSortingNetwork();

        IComparatorNetwork CloneWithNewComparator(Comparator comparator);
    }

    [Serializable]
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
