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

        static long RedundantNumber { get; set; }
#endif
        static int NumComparators { get; set; }
        static int Inputs { get; set; }

        static int OutputSize => (int)Math.Max((1 << Inputs) / 32, 1);

        int[] Outputs { get; }
#if DUAL
        int[] OutputsDual { get; }
#endif
        int[] Where0SetCount { get; }

        int[] SequencesWithOnes { get; }

        int[] Where0 { get; }

        int[] Where1 { get; }

        Comparator[] Comparators { get; set; }

        int OutputsPopCount { get; }

        bool IsSubsumed(IComparatorNetwork n);

        bool IsRedundant(IComparatorNetwork n);

        bool IsSortingNetwork();

        float BadZeroesHeuristic();

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
