using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SortingNetworks
{
    public class GraphMatchesFinder
    {
        private static int V { get; set; }


        private bool BPM(bool[,] bpGraph, int u, bool[] seen, int[] matchR)
        {
            for (var v = 0; v < bpGraph.GetLength(0); v++)
            {
                if (!bpGraph[u, v] || seen[v]) continue;

                seen[v] = true;

                if (matchR[v] < 0 || BPM(bpGraph, matchR[v], seen, matchR))
                {
                    matchR[v] = u;
                    return true;
                }
            }
            return false;
        }

        // Returns maximum number of  

        // matching from M to N 

        public int[] FindPerfectMatch(bool[,] bpGraph)
        {
            var dimension = bpGraph.GetLength(0);
            var matchR = new int[dimension];

            for (var i = 0; i < dimension; ++i) matchR[i] = -1;

            for (var u = 0; u < dimension; u++)
            {
                var seen = new bool[dimension];
                for (var i = 0; i < dimension; ++i)
                    seen[i] = false;

                BPM(bpGraph, u, seen, matchR);
            }

            return matchR;
        }
    }
}
