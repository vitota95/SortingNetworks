namespace SortingNetworks
{
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IPruner"/>
    public class Pruner : IPruner
    {
        private static readonly IEnumerable<int>[] Permutations = Enumerable.Range(0, IComparatorNetwork.Inputs).GetPermutations().ToArray();

        /// <inheritdoc cref="IPruner.Prune"/>
        public IReadOnlyList<IComparatorNetwork> Prune(IReadOnlyList<IComparatorNetwork> nets)
        {
            var result = new List<IComparatorNetwork>();

            for (var i = 0; i < nets.Count; i++) 
            {
                var isSubsumed = false;

                for (var index = result.Count - 1; index >= 0; index--)
                {
                    var n = result[index];

                    if (nets[i].IsSubsumed(n, Permutations))
                    {
                        isSubsumed = true;
                        break;
                    }

                    if (n.IsSubsumed(nets[i], Permutations))
                    {
                        result.Remove(n);
                    }
                }

                if (!isSubsumed) 
                {
                    result.Add(nets[i]);
                }
            }

            return result;
        }

        public IReadOnlyList<IComparatorNetwork> Remove(IReadOnlyList<IComparatorNetwork> nets1, IReadOnlyList<IComparatorNetwork> nets2)
        {
            var result = nets2.ToList();

            for (var i = result.Count - 1; i >= 0; i--)
            {
                for (var j = 0; j < nets1.Count; j++)
                {
                    if (nets2[i].IsSubsumed(nets1[j], Permutations))
                    {
                        result.Remove(nets2[i]);
                    }
                }
            }

            return result;
        }
    }
}
