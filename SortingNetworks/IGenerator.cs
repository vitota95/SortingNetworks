namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IGenerator
    {
        IReadOnlyList<IComparatorNetwork> Generate(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators);
    }
}
