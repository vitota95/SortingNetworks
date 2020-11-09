using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;

namespace SortingNetworksTests
{
    [TestClass]
    public class GraphMatchesFinderTests
    {
        [TestMethod]
        public void FindPerfectMatches_Test()
        {
            // Arrange
            var positions = new[] {3, 17, 12, 17, 10};

            // Act
            var result = GraphMatchesFinder.FindPerfectMatches(positions);

            // Assert
            result.Should().ContainInOrder(new int[] { 1, 4, 2, 0, 3 });
        }
    }
}
