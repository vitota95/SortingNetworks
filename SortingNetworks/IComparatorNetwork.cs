using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SortingNetworks
{
    public interface IComparatorNetwork
    {
        short Size { get; }
        Tuple<short, short>[] Comparators { get; set; }
        HashSet<short> Outputs
        {
            get;
        }
        bool IsEquivalent(IComparatorNetwork n);
        bool IsSortingNetwork();
    }
}
