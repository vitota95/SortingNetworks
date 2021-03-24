using System;
using System.Collections.Generic;
using System.Linq;

namespace SortingNetworks.Parallel
{
    public class GeneratePruneInBatches : IBatchesGeneratorPruner
    {
        public int BatchSize { get; }

        public GeneratePruneInBatches(int batchSize)
        {
            BatchSize = batchSize;
        }

        public IReadOnlyList<IComparatorNetwork> GeneratePrune(IReadOnlyList<IComparatorNetwork> nets, IList<Comparator> comparators)
        {
            var generator = new Generator();
            var pruner = new ParallelPruner();
            var result = new List<IComparatorNetwork>();

            if (nets.Count > this.BatchSize)
            {
                var batches = nets.SplitList(this.BatchSize).ToList();
                List<IReadOnlyList<IComparatorNetwork>> splitNets;
                foreach (var batch in batches)
                { 
                    var generatedNets = generator.Generate(batch, comparators);
                    // use all threads in split, size is supposed to be big enough at this point
                    splitNets = generatedNets.SplitList((generatedNets.Count / IPruner.Threads) + 1).ToList();
                    result.AddRange(pruner.Prune(splitNets));
                }

                // shuffle result??
                result = result.OrderBy(c => Guid.NewGuid()).ToList();
                splitNets = result.SplitList((result.Count / IPruner.Threads) + 1).ToList();
                result = pruner.Prune(splitNets).ToList();
            }
            else
            {
                var generatedNets = generator.Generate(nets, comparators);
                var splitNets = generatedNets.SplitList((generatedNets.Count / IPruner.Threads) + 1).ToList();
                result.AddRange(pruner.Prune(splitNets));
            }

            return result;
        }
    }
}
