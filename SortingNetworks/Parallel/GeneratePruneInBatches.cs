using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                foreach (var batch in batches)
                { 
                    var generatedNets = generator.Generate(batch, comparators);
                    // use all threads in split, size is supposed to be big enough at this point
                    var splitNets = generatedNets.SplitList((generatedNets.Count / IPruner.Threads) + 1).ToList();
                    result.AddRange(pruner.Prune(splitNets));
                }

                result = pruner.Prune(result).ToList();
            }
            else
            {
                var generatedNets = generator.Generate(nets, comparators);
                // use at least 2000 nets per thread
                var splitNets = generatedNets.SplitList(Math.Max(generatedNets.Count / Math.Max(IPruner.Threads, (short)1) + 1, 2000)).ToList();
                result.AddRange(pruner.Prune(splitNets));
            }

            return result;
        }
    }
}
