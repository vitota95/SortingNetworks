﻿using System;
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
        public IReadOnlyList<IComparatorNetwork> Prune(IReadOnlyList<IReadOnlyList<IComparatorNetwork>> nets)
        {
            var pruner = new Pruner();
            var netsAfterPrune = new IReadOnlyList<IComparatorNetwork>[nets.Count];

            var tasks = Enumerable.Range(0, nets.Count)
                .Select(i => Task.Run(() =>
                {
                    var net = nets[i].ToList();
                    netsAfterPrune[i] = pruner.Prune(net);
                })).ToArray();

            Task.WaitAll(tasks);

            for (var i = 0; i < nets.Count; i++)
            {
                var index = i;
                var removeTasks = Enumerable.Range(0, nets.Count)
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
            
            return netsAfterPrune
                .SelectMany(x => x).ToList();
        }
    }
}
