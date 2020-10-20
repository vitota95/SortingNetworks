using System.Collections.Generic;

namespace SortingNetworks
{
    public class ComparatorNetworksGenerator : IComparatorNetworksGenerator
    {
        public IReadOnlyList<IComparatorNetwork> Generate(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators)
        {
            var newSet = new List<IComparatorNetwork>();
            for (var i = 0; i < nets.Count; i++)
            {
                var net = nets[i];
                for (var j = 0; j < comparators.Count; j++)
                {
                    var newNet = net.CloneWithNewComparator(comparators[j]);
                    var isRedundant = newNet.IsRedundant(net);
                    
                    if (!isRedundant)
                    {
                        newSet.Add(newNet);
                    }
                }
            }
            return newSet.ToArray();
        }    
    }
}
