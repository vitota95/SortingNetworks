using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using System;

namespace SortingNetworksTests
{
    [TestClass]
    public class ComparatorNetworkTests
    {
        [TestMethod]
        public void Output_WithSize2And1Comparator()
        {
            // Arrange, Act
            short size = 2;
            var comparators = new Tuple<short, short>[1] { new Tuple<short, short>(0, 1) };
            var s1 = new ComparatorNetwork(size,  comparators);

            // Assert
            Assert.AreEqual(size + 1, s1.Output.Count);
        }
    }
}
