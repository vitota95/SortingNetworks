﻿namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
        static ushort Inputs { get; set; }

        Comparator[] Comparators { get; set; }

        HashSet<ushort> Outputs { get; }

        Dictionary<ushort, int> Where0 { get; }

        Dictionary<ushort, int> SequencesWithOnes { get; }

        Dictionary<ushort, int> Where1 { get; }

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
