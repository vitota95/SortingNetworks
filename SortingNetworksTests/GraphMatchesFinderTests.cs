//using System;
//using System.Collections.Generic;
//using System.Text;
//using FluentAssertions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SortingNetworks;

//namespace SortingNetworksTests
//{
//    [TestClass]
//    public class GraphMatchesFinderTests
//    {
//        [TestMethod]
//        public void FindPerfectMatches_WhenGraphContainsAPerfectMatch()
//        {
//            // Arrange
//            var sut = new GraphMatchesFinder();

//            // Act
//            var result = sut.FindPerfectMatch(new[] { 1, 2, 4, 8, 16 });

//            // Assert
//            result.Should().ContainInOrder(new int[] { 0, 1, 2, 3, 4 });
//        } 
        
//        [TestMethod]
//        public void FindPerfectMatches_WhenGraphContainsNoPerfectMatch()
//        {
//            // Arrange
//            var sut = new GraphMatchesFinder();

//            // Act
//            var result = sut.FindPerfectMatch(new[] { 0, 2, 4, 8, 16 });

//            // Assert
//            result.Should().BeNull();
//        }
//    }
//}
