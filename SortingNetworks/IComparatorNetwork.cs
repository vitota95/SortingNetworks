#define DUAL

using System.Text.Json.Serialization;

namespace SortingNetworks
{
    using System;
    using System.Collections.Generic;

    public interface IComparatorNetwork
    {
#if DEBUG
        static long SubsumeTotal { get; set; }
        static long SubsumeNoCheck1 { get; set; }
        static long SubsumeNoCheck2 { get; set; }
        static long SubsumeNoCheckTotal { get; set; }
        static long OutputCountBigger { get; set; }
        static long PermutationsNumber { get; set; }
        static long PermutationsWalk { get; set; }
        static long SubsumeNumber { get; set; }
        static long SubsumeSucceed{ get; set; }
        static long IsSubset { get; set; }
        static long IsSubsetDual { get; set; }
        static long TryPermutationCall { get; set; }
#endif

#if DUAL
        static int NumComparators { get; set; }

        int[] OutputsDual { get; }

        int[] Where0Dual { get; }

        int[] Where1Dual { get; }
#endif
        static int Inputs { get; set; }

        static int OutputSize => (int) Math.Max((1 << Inputs) / 32, 1);

        Comparator[] Comparators { get; set; }

        int[] Outputs { get; }

        int OutputsPopCount { get; }

        int[] Where0 { get; }

        int[] Where0SetCount { get; }

        int[] SequencesWithOnes { get; }

        int[] Where1 { get; }

        bool IsSubsumed(IComparatorNetwork n);

        bool IsRedundant(IComparatorNetwork n);

        bool IsSortingNetwork();

        IComparatorNetwork CloneWithNewComparator(Comparator comparator);

        bool IsSortingNetwork2N();
    }

    [Serializable]
    public struct Comparator
    {
        [JsonInclude]
        public int X;

        [JsonInclude]
        public int Y;

        public Comparator(int x, int y) 
        {
            this.X = x;
            this.Y = y;
        }
    }
}
