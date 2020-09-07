using System;
using System.Collections;
using System.Collections.Generic;

namespace SortingNetworks
{
    public interface IComparatorNetwork
    {
        short Size { get; }
        Tuple<short, short>[] Comparators { get; set; }
        HashSet<BitArray> Output
        {
            get;
        }
        bool IsEquivalent(IComparatorNetwork n);
    }
}
