namespace SortingNetworks
{
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IPruner"/>
    public class Pruner : IPruner
    {
        /// <inheritdoc cref="IPruner.Prune"/>
        public IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            var result = new List<IComparatorNetwork>();
            var range = Enumerable.Range(0, nets[0].Inputs);
            var permutations = range.GetPermutations().ToArray();

            for (var i = 0; i < nets.Length; i++) 
            {
                var isSubsumed = false;

                for (var index = result.Count - 1; index >= 0; index--)
                {
                    var n = result[index];

                    if (nets[i].IsSubsumedBy(n, permutations))
                    {
                        isSubsumed = true;
                        break;
                    }

                    if (n.IsSubsumedBy(nets[i], permutations))
                    {
                        result.Remove(n);
                    }
                }

                if (!isSubsumed) 
                {
                    result.Add(nets[i]);
                }
            }

            return result.ToArray();
        }
    }
}
