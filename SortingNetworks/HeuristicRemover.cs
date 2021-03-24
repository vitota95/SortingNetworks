using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Numerics.BitOperations;

namespace SortingNetworks
{
    public class HeuristicRemover
    {
        public static IReadOnlyList<IComparatorNetwork> RemoveNetsWithMoreOutputs(IReadOnlyList<IComparatorNetwork> nets, int netsToKeep = 15000)
        {
            return nets.OrderBy(x => x.OutputsPopCount)
                        .Take(netsToKeep)
                        .ToList();
        }
        
        public static IReadOnlyList<IComparatorNetwork> RemoveNetsWithMoreBadZeroes(IReadOnlyList<IComparatorNetwork> nets, int netsToKeep = 15000)
        {
            return nets.OrderBy(x => x.BadZeroesHeuristic())
                        .Take(netsToKeep)
                        .ToList();
        }
    }
}
