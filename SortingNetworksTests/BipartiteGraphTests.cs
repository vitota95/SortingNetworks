using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks.Graphs;

namespace SortingNetworksTests
{
    [TestClass]
    public class BipartiteGraphTests
    {
        [TestMethod]
        public void FindPerfectMatch_test()
        {
            BipartiteGraphMatching.GetHopcroftKarpMatching(new[] {6, 7, 6, 24, 24});
        }

        [TestMethod]
        public void GetCycle_test()
        {
            BipartiteGraphMatching.GetCycle(new[] {6, 7, 6, 24, 24});
        }
    }
}
