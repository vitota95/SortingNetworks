using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingNetworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SortingNetworksTests
{
    [TestClass]
    public class ComparatorNetworkTests
    {
        [TestMethod]
        public void IsSortingNetwork_WithSize2And1Comparator_ReturnsTrue()
        {
            // Arrange, Act
            int size = 2;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And3Comparators_ReturnsTrue()
        {
            // Arrange, Act
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[3] { new Comparator(0, 2), new Comparator(0, 1), new Comparator(1, 2) };
            var s1 = new ComparatorNetwork(comparators);

            // Assert
            Assert.IsTrue(s1.IsSortingNetwork());
        }
        
        [TestMethod]
        public void IsSortingNetwork_WithSize3And3Comparators_ReturnsFalse()
        {
            // Arrange, Act
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[3] { new Comparator(1, 2), new Comparator(0, 1), new Comparator(0, 2) };
            var s1 = new ComparatorNetwork(comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And2Comparators_ReturnsFalse()
        {
            // Arrange, Act
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[2] { new Comparator(0, 2), new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }

        [TestMethod]
        public void IsSortingNetwork_WithSize3And1Comparator_ReturnsFalse()
        {
            // Arrange, Act
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(comparators);

            // Assert
            Assert.IsFalse(s1.IsSortingNetwork());
        }      
        
        [TestMethod]
        public void CloneWithNewComparator_When0Comparator_Contains1Comparator()
        {
            // Arrange
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[0];
            var s1 = new ComparatorNetwork(comparators);

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
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var comparators = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(comparators);

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
            int size = 2;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[1] { new Comparator(0, 1) };
            var s1 = new ComparatorNetwork(c1);
            var s2 = new ComparatorNetwork(c2);

            // Act
            var result = s2.IsRedundant(s1);

            // Assert
            Assert.IsTrue(result);          
        }
        
        [TestMethod]
        public void IsRedundant_WhenAreNotRedundant_IsMarkedIsFalse()
        {
            // Arrange
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var c2 = new Comparator[1] { new Comparator(0, 2) };
            var s1 = new ComparatorNetwork(c1);
            var s2 = new ComparatorNetwork(c2);

            // Act
            var result = s2.IsRedundant(s1);

            // Assert
            Assert.IsFalse(result);          
        }

        [TestMethod]
        public void Output_When3Inputs1Comparator_HasExpectedResult()
        {
            // Arrange, Act
            int size = 3;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[1] { new Comparator(0, 1) };
            var n = new ComparatorNetwork(c1);

            // Assert
            //Assert.AreEqual(4, n.Outputs.Count);
            //Assert.IsTrue(n.Outputs.SetEquals(new HashSet<int> { 1, 4, 5, 6 }));
        }

        [TestMethod]
        public void SubsumePositiveTest()
        {
            // Arrange, Act
            int size = 4;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[3] { new Comparator(0, 1), new Comparator(0, 3), new Comparator(1,2)  };
            var c2 = new Comparator[3] { new Comparator(0, 1), new Comparator(0, 3), new Comparator(2,3) };

            var n1 = new ComparatorNetwork(c1);
            var n2 = new ComparatorNetwork(c2);

            var result = n1.IsSubsumed(n2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GenerateOutputs()
        {
            // Arrange, Act
            int size = 4;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[3] { new Comparator(0, 1), new Comparator(0, 2), new Comparator(2, 3) };
            var c2 = new Comparator[3] { new Comparator(0, 1), new Comparator(0, 3), new Comparator(1, 3) };
            var c3 = new Comparator[3] { new Comparator(1, 2), new Comparator(2, 3), new Comparator(0, 3) };

            Console.WriteLine("c1 outputs");
            var n1 = new ComparatorNetwork(c1);
            Console.WriteLine("c2 outputs");
            var n2 = new ComparatorNetwork(c2);
            Console.WriteLine("c3 outputs");
            var n3 = new ComparatorNetwork(c3);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PrintWhereMatrix()
        {
            // Arrange, Act
            int size = 4;
            IComparatorNetwork.Inputs = size;
            var c1 = new Comparator[3] { new Comparator(2, 4), new Comparator(2, 3), new Comparator(1, 3) };
            var c2 = new Comparator[3] { new Comparator(1, 4), new Comparator(3, 4), new Comparator(1, 3) };

            Trace.WriteLine("c1 outputs");
            var n1 = new ComparatorNetwork(c1);
            Trace.WriteLine("c2 outputs");
            var n2 = new ComparatorNetwork(c2);

            n1.PrintWhereMatrix(n2);
            n2.PrintWhereMatrix(n1);

            Assert.IsTrue(true);
        }
    }
}
