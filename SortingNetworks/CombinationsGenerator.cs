using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public class CombinationsGenerator : ICombinationsGenerator
    {
        public IList<IList<T>> GenerateCombinations<T>(IList<T> combinationList, int k)
        {
            var combinations = new List<IList<T>>();

            if (k == 0)
            {
                var emptyCombination = new List<T>();
                combinations.Add(emptyCombination);

                return combinations;
            }

            if (combinationList.Count == 0)
            {
                return combinations;
            }

            T head = combinationList[0];
            var copiedCombinationList = new List<T>(combinationList);

            var subcombinations = GenerateCombinations(copiedCombinationList, k - 1);

            foreach (var subcombination in subcombinations)
            {
                subcombination.Insert(0, head);
                combinations.Add(subcombination);
            }

            combinationList.RemoveAt(0);
            combinations.AddRange(GenerateCombinations(combinationList, k));

            return combinations;
        }
    }
}
