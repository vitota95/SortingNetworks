using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public interface IComparatorsGenerator
    {
        IList<Comparator> GenerateComparators(int[] range);
    }
}
