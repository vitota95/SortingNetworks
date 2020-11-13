using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using SortingNetworks.Graphs;

namespace SortingNetworksTests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void Graph_GetCycle_IsCorrect()
        {
            // Arrange
            IComparatorNetwork.Inputs = 2;
            var sut = new Graph(new [] {2, 1});
            
            // Act
            var result = sut.GetCycle();

            // Assert
            result.Should().NotBeNull();
        } 
        
        [TestMethod]
        public void GetCycle_WhenNoCycle_ReturnsNull()
        {
            // Arrange
            IComparatorNetwork.Inputs = 2;
            var sut = new Graph(new [] {0, 1});
            
            // Act
            var result = sut.GetCycle();

            // Assert
            result.Should().BeNull();
        } 
        
        [TestMethod]
        public void GetCycle_WhenNoCycle3Inputs_ReturnsNull()
        {
            // Arrange
            IComparatorNetwork.Inputs = 3;
            var sut = new Graph(new [] {2, 0, 1});
            
            // Act
            var result = sut.GetCycle();

            // Assert
            result.Should().BeNull();
        } 
        
        [TestMethod]
        public void RemoveEdges_WhenEdgeExist()
        {
            // Arrange
            IComparatorNetwork.Inputs = 3;
            var sut = new Graph(new [] {1, 2, 4});

            // Act
            sut.RemoveEdges(new[] { new Edge(0, 0), new Edge(1, 1) });

            // Assert
            sut.Edges.Should().HaveCount(1);
        }
    }
}
