namespace SortingNetworks
{
    using System.Collections.Generic;

    /// <inheritdoc cref="IPruner"/>
    public class Pruner : IPruner
    {
        /// <inheritdoc cref="IPruner.Prune"/>
        public IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            var result = new List<IComparatorNetwork>();
            for (var i = 0; i < nets.Length; i++) 
            {
                var isSubsumed = false;

                for (var index = result.Count - 1; index >= 0; index--)
                {
                    var n = result[index];

                    if (nets[i].IsSubsumed(n))
                    {
                        isSubsumed = true;
                        break;
                    }

                    if (n.IsSubsumed(nets[i]))
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
