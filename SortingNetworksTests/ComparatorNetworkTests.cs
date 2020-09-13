using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using System;
using System.Collections.Generic;

namespace SortingNetworksTests
{
    [TestClass]
    public class ComparatorNetworkTests
    {
        [TestMethod]
        public void IsSortingNetwork_WithSize2And1Comparator_ReturnsTrue()
        {
            // Arrange, Act
            short size = 2;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size,  comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And3Comparators_ReturnsTrue()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[3] { new Comparator(0, 2), new Comparator(0, 1), new Comparator(1, 2) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }
        
        [TestMethod]
        public void IsSortingNetwork_WithSize3And3Comparators_ReturnsFalse()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[3] { new Comparator(1, 2), new Comparator(0, 1), new Comparator(0, 2) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And2Comparators_ReturnsFalse()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[2] { new Comparator(0, 2), new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And1Comparator_ReturnsFalse()
        {
            // Arrange, Act
            short size = 3;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }      
        
        [TestMethod]
        public void CloneWithNewComparator_When0Comparator_Contains1Comparator()
        {
            // Arrange
            short size = 3;
            var comparators = new Comparator[0];
            var s1 = new ComparatorNetwork(size, comparators);

            // Act
            var s2 = s1.CloneWithNewComparator(new Comparator(1, 2));

            // Assert
            Assert.AreEqual(1, s2.Comparators.Length);
            Assert.AreEqual(1, s2.Comparators[0].X);
            Assert.AreEqual(2, s2.Comparators[0].Y); 
        }

        [TestMethod]
        public void CloneWithNewComparator_When1Comparator_Contains2Comparators()
        {
            // Arrange
            short size = 3;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, comparators);

            // Act
            var s2 = s1.CloneWithNewComparator(new Comparator(1, 2));

            // Assert
            Assert.AreEqual(2, s2.Comparators.Length);
            Assert.AreEqual(0, s2.Comparators[0].X);
            Assert.AreEqual(1, s2.Comparators[0].Y);
            Assert.AreEqual(1, s2.Comparators[1].X);
            Assert.AreEqual(2, s2.Comparators[1].Y);
        }

        [TestMethod]
        public void IsRedundant_WhenAreRedundant_IsMarkedIsTrue()
        {
            // Arrange
            short size = 2;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(size, c1);
            var s2 = new ComparatorNetwork(size, c2);

            // Act
            var result = s2.IsRedundant(s1);

            // Assert
            Assert.IsTrue(result);          
        }
        
        [TestMethod]
        public void IsRedundant_WhenAreNotRedundant_IsMarkedIsFalse()
        {
            // Arrange
            short size = 3;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[1] { new Comparator(0, 2) };
            var s1 = new ComparatorNetwork(size, c1);
            var s2 = new ComparatorNetwork(size, c2);

            // Act
            var result = s2.IsRedundant(s1);

            // Assert
            Assert.IsFalse(result);          
        }

        [TestMethod]
        public void Output_When3Inputs1Comparator_HasExpectedResult()
        {
            // Arrange, Act
            short size = 3;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var n = new ComparatorNetwork(size, c1);

            // Assert
            Assert.AreEqual(4, n.Outputs.Count);
            Assert.IsTrue(n.Outputs.SetEquals(new HashSet<short> { 1, 4, 5, 6 }));
        }
    }
}
