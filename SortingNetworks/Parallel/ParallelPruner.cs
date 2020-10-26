using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SortingNetworks.Parallel
{
    public class ParallelPruner
    {
        public IReadOnlyList<IComparatorNetwork> Prune(IReadOnlyList<IComparatorNetwork> nets, int numTasks)
        {
            var pruner = new Pruner();
            var splitNets = nets.SplitList().ToList();
            var netsAfterPrune = new IReadOnlyList<IComparatorNetwork>[numTasks];

            var tasks = Enumerable.Range(0, splitNets.Count)
                .Select(i => Task.Run(() =>
                {
                    var net = splitNets[i].ToList();
                    netsAfterPrune[i] = pruner.Prune(net);
                })).ToArray();

            Task.WaitAll(tasks);

            for (var i = 0; i < splitNets.Count; i++)
            {
                var index = i;
                var removeTasks = Enumerable.Range(0, splitNets.Count)
                    .Select(j => Task.Run(() =>
                    {
                        var s1 = netsAfterPrune[index];
                        if (s1 != null && index != j)
                        {
                            var s2 = netsAfterPrune[j];
                            netsAfterPrune[j] = pruner.Remove(s1, s2);
                        }

                    })).ToArray();

                Task.WaitAll(removeTasks);
            }
            
            return netsAfterPrune.Where(n => n != null)
                .SelectMany(x => x).Distinct().ToList();
        }
    }
}
