namespace SortingNetworks
{
    using System.Collections.Generic;

    public interface IComparatorsGenerator
    {
        IList<Comparator> GenerateComparators(int[] range);
    }
}
