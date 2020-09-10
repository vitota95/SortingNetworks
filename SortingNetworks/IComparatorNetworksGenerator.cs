namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetworksGenerator
    {
        IComparatorNetwork[] Generate(IComparatorNetwork[] nets, IList<Comparator> comparators);
    }
}
