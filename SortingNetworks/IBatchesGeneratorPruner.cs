using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingNetworks
{
    public interface IBatchesGeneratorPruner
    {
        IReadOnlyList<IComparatorNetwork> GeneratePrune(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators);
    }
}
