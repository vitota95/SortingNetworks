namespace SortingNetworksTests
{
    using System;

    using AutoFixture;
    using AutoFixture.AutoMoq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SortingNetworks;

    [TestClass]
    public class SortingNetworksGeneratorTests
    {
        [TestMethod]
        public void Generate_WithRedundantComparator_NotAddNewNet()
        {
            // Arrange
            const ushort Size = 2;
            var comparators = new Comparator[] { new Comparator(0, 1), new Comparator(0, 1) };
            var generator = new ComparatorNetworksGenerator();
            IComparatorNetwork[] nets = { new ComparatorNetwork(Size, comparators) };

            // Act
            var result = generator.Generate(nets, comparators);

            // Assert
            Assert.AreEqual(0, result.Length);
        }
        
        [TestMethod]
        public void Generate_WithNonRedundantComparator_AddsNewNets()
        {
            // Arrange
            const ushort Size = 3;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[2] { new Comparator(0, 2), new Comparator(1, 2) };
            var generator = new ComparatorNetworksGenerator();
            IComparatorNetwork[] nets = { new ComparatorNetwork(Size, c1) };

            // Act
            var result = generator.Generate(nets, c2);

            // Assert
            Assert.AreEqual(2, result.Length);
        }
        
        [TestMethod]
        public void Generate_WithSize3And3ComparatorsNonRedundancyCheck_MultipliesBy3EachIteration()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var comparators = new[] { new Comparator(0, 1), new Comparator(0, 2), new Comparator(1, 2) };
            var generator = new ComparatorNetworksGenerator();
            var netMock = fixture.Freeze<IComparatorNetwork>();
            var nets = new[] { netMock };

            // Act, Assert
            for (var i = 0; i < comparators.Length; i++)
            {
                nets = generator.Generate(nets, comparators);
                Assert.AreEqual(Math.Pow(3, i + 1), nets.Length);
            }
        }
    }
}