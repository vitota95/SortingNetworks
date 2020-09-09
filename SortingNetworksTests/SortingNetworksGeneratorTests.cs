using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworksTests
{
    [TestClass]
    public class SortingNetworksGeneratorTests
    {
        [TestMethod()]
        public void Generate_WithRedundantComparator_NotAddNewNet()
        {
            // Arrange
            short size = 2;
            var comparators = new Comparator[] { new Comparator(0, 1), new Comparator(0, 1) };
            var nets = new ComparatorNetwork[] { new ComparatorNetwork(size, comparators) };
            var generator = new SortingNetworksGenerator();

            // Act
            var result = generator.Generate(nets, comparators);

            // Assert
            Assert.AreEqual(1, result.Length);
        }
        
        [TestMethod()]
        public void Generate_WithNonRedundantComparator_AddsNewNet()
        {
            // Arrange
            short size = 3;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[2] { new Comparator(0, 1), new Comparator(0, 2) };
            var nets = new ComparatorNetwork[] { new ComparatorNetwork(size, c1) };
            var generator = new SortingNetworksGenerator();

            // Act
            var result = generator.Generate(nets, c2);

            // Assert
            Assert.AreEqual(2, result.Length);
        }
    }
}