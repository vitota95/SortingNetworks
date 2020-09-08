using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public interface ICombinationsGenerator
    {
        IList<IList<T>> GenerateCombinations<T>(IList<T> combinationList, int k);
    }
}
