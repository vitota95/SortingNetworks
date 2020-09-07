using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using System;

namespace SortingNetworksTests
{
    [TestClass]
    public class ComparatorNetworkTests
    {
        [TestMethod]
        public void Output_WithSize2And1Comparator_IsSortingNetwork()
        {
            // Arrange, Act
            short size = 2;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size,  comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void Output_WithSize3And3Comparators_IsSortingNetwork()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[3] { new Comparator(0, 2), new Comparator(0, 1), new Comparator(1, 2) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }
        
        [TestMethod]
        public void Output_WithSize3And3Comparators_NotSortingNetwork()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[3] { new Comparator(1, 2), new Comparator(0, 1), new Comparator(0, 2) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void Output_WithSize3And2Comparators_NotSortingNetwork()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[2] { new Comparator(0, 2), new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void Output_WithSize3And1Comparator_NotSortingNetwork()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }
    }
}
