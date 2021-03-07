using System.Diagnostics;

namespace SortingNetworks
{
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IPruner"/>
    public class Pruner : IPruner
    {
        /// <inheritdoc cref="IPruner.Prune"/>
        public IReadOnlyList<IComparatorNetwork> Prune<T>(IReadOnlyList<T> nets)
        {
            var result = new List<IComparatorNetwork>();

            for (var i = 0; i < nets.Count; i++) 
            {
                var isSubsumed = false;
                var n1 = nets[i] as IComparatorNetwork;

                for (var index = result.Count - 1; index >= 0; index--)
                {
                    var n2 = result[index];
                    if (n1.IsSubsumed(n2))
                    {
                        //Trace.WriteLine("Subsumed");
                        isSubsumed = true;
                        break;
                    }

                    if (n2.IsSubsumed(n1))
                    {
                        //Trace.WriteLine("Subsumed2");
                        result.Remove(n2);
                    }
                }

                if (!isSubsumed) 
                {
                    result.Add(n1);
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
                    if (nets2[i].IsSubsumed(nets1[j]))
                    {
                        result.Remove(nets2[i]);
                    }
                }
            }

            return result;
        }
    }
}
