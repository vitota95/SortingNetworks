namespace SortingNetworks
{
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IPruner"/>
    public class Pruner : IPruner
    {
        /// <inheritdoc cref="IPruner.Prune"/>
        public IReadOnlyList<IComparatorNetwork> Prune(IReadOnlyList<IComparatorNetwork> nets)
        {
            var result = new List<IComparatorNetwork>();
            var permutations = Enumerable.Range(0, IComparatorNetwork.Inputs).GetPermutations().ToArray();

            for (var i = 0; i < nets.Count; i++) 
            {
                var isSubsumed = false;

                for (var index = result.Count - 1; index >= 0; index--)
                {
                    var n = result[index];

                    if (nets[i].IsSubsumed(n, permutations))
                    {
                        isSubsumed = true;
                        break;
                    }

                    if (n.IsSubsumed(nets[i], permutations))
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
    }
}
