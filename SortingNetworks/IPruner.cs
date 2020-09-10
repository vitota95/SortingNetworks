using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public interface IPruner
    {
        IComparatorNetwork[] Prune(IComparatorNetwork[] nets);
    }
}
