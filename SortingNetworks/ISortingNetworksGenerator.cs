using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public interface ISortingNetworksGenerator
    {
        IComparatorNetwork[] Generate(IComparatorNetwork[] nets, IList<Comparator> comparators);
    }
}
