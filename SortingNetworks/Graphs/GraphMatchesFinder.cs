using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        public int[] FindPerfectMatch(int[] bpGraph)
        {
            var dimension = bpGraph.Length;
            var matchR = new int[dimension];

            for (var i = 0; i < dimension; ++i) matchR[i] = -1;

            for (var u = 0; u < dimension; u++)
            {
                var seen = new bool[dimension];
                for (var i = 0; i < dimension; ++i)
                    seen[i] = false;

                BPM(bpGraph, u, seen, matchR);
            }

            return matchR.Contains(-1) ? null : matchR;
        }

        private static bool BPM(int[] bpGraph, int u, bool[] seen, int[] matchR)
        {
            for (var v = 0; v < bpGraph.Length; v++)
            {
                if ((bpGraph[u] & (1 << v)) == 0 || seen[v]) continue;

                seen[v] = true;
                
                if (matchR[v] < 0 || BPM(bpGraph, matchR[v], seen, matchR))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }
    }
}
