using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SortingNetworks
{
    public class Generator : IGenerator
    {
        public IReadOnlyList<IComparatorNetwork> Generate(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators)
        {
            var newSet = new ConcurrentBag<IComparatorNetwork>();

            System.Threading.Tasks.Parallel.For(0, nets.Count, index =>
            {
                for (var j = 0; j < comparators.Count; j++)
                {
                    var net = nets[index];
                    var newNet = net.CloneWithNewComparator(comparators[j]);
                    var isRedundant = newNet.IsRedundant(net);

                    if (!isRedundant)
                    {
                        newSet.Add(newNet);
                    }
                }
            });

            if (newSet.IsEmpty && nets.Any(x => x.IsSortingNetwork()))
            {
                Trace.WriteLine("Set already contains a sorting network!");
                return nets;
            }
          
            return newSet.ToArray();
        }    
    }
}
