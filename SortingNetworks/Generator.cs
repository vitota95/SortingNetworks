using System.Collections.Generic;

namespace SortingNetworks
{
    public class SortingNetworksGenerator : ISortingNetworksGenerator
    {
        public IComparatorNetwork[] Generate(IComparatorNetwork[] nets, IList<Comparator> comparators)
        {
            var newSet = new List<IComparatorNetwork>();
            for (var i = 0; i < nets.Length; i++)
            {
                var net = nets[i];
                for (var j = 0; j < comparators.Count; j++)
                {
                    var newNet = net.CloneWithNewComparator(comparators[j]);

                    foreach(var n in newSet) 
                    {
                        if (newNet.IsRedundant(n)) break;                       
                    }

                    if (!newNet.IsMarked) 
                    {
                        newSet.Add(newNet);
                    }
                }
            }

            return newSet.ToArray();
        }    
    }
}
