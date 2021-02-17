using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using SortingNetworks.Graphs;

namespace SortingNetworksTests
{
    [TestClass]
    public class BipartiteGraphTests
    {
        [TestMethod]
        public void FindPerfectMatch_test()
        {
            IComparatorNetwork.Inputs = 5;
            var graphMatcher = new BipartiteGraphMatching();

            graphMatcher.GetHopcroftKarpMatching(new[] {6, 7, 6, 24, 24});
        }

        [TestMethod]
        public void GetAllPerfectMatchings_test()
        {
            IComparatorNetwork.Inputs = 5;
            var graphMatcher = new BipartiteGraphMatching();

            graphMatcher.GetAllPerfectMatchings(new[] {6, 7, 6, 24, 24});
        }
    }
}
