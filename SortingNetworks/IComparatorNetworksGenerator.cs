namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorNetworksGenerator
    {
        IReadOnlyList<IComparatorNetwork> Generate(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators);
    }
}
