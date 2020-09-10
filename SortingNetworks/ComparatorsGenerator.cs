namespace SortingNetworks
{
    using System.Collections.Generic;

    public class ComparatorsGenerator : IComparatorsGenerator
    {
        public IList<Comparator> GenerateComparators(int[] range)
        {
            var comparators = new List<Comparator>();

            for (short i = 0; i < range.Length; i++)
            {
                for (var j = (short)(1 + i); j < range.Length; j++)
                {
                    comparators.Add(new Comparator(i, j));
                }
            }

            return comparators;
        }
    }
}
