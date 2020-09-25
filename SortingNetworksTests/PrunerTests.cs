namespace SortingNetworksTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;

    using AutoFixture;
    using AutoFixture.AutoMoq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SortingNetworks;

    [TestClass]
    public class PrunerTests
    {
        [TestMethod]
        public void Prune_WhenNoSubsumptions_ReturnsSameArray()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var nets = fixture.CreateMany<IComparatorNetwork>(3).ToArray();
            var pruner = new Pruner();

            // Act
            var result = pruner.Prune(nets);

            // Assert
            Assert.AreEqual(nets.Length, result.Length);
        }

        [TestMethod]
        public void Prune_When1SubsumptionInFirstElement_Prunes1Element()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var netMocks = fixture.CreateMany<Mock<IComparatorNetwork>>(3).ToArray();
            var pruner = new Pruner();
            netMocks[0].Setup(x => x.IsSubsumedBy(netMocks[1].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);

            var nets = netMocks.Select(x => x.Object).ToArray();

            // Act
            var result = pruner.Prune(nets);

            // Assert
            Assert.AreEqual(2, result.Length);
        }  
        
        [TestMethod]
        public void Prune_When1SubsumptionInSecondElement_Prunes1Element()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var netMocks = fixture.CreateMany<Mock<IComparatorNetwork>>(3).ToArray();
            var pruner = new Pruner();
            netMocks[1].Setup(x => x.IsSubsumedBy(netMocks[0].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);

            var nets = netMocks.Select(x => x.Object).ToArray();

            // Act
            var result = pruner.Prune(nets);

            // Assert
            Assert.AreEqual(2, result.Length);
        }
        
        [TestMethod]
        public void Prune_WhenAllAreSubsumed_ReturnsFirstElement()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var netMocks = fixture.CreateMany<Mock<IComparatorNetwork>>(3).ToArray();
            var pruner = new Pruner();
            netMocks[0].Setup(x => x.IsSubsumedBy(netMocks[1].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);
            netMocks[0].Setup(x => x.IsSubsumedBy(netMocks[2].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);
            netMocks[1].Setup(x => x.IsSubsumedBy(netMocks[0].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);
            netMocks[1].Setup(x => x.IsSubsumedBy(netMocks[2].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);
            netMocks[2].Setup(x => x.IsSubsumedBy(netMocks[0].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);
            netMocks[2].Setup(x => x.IsSubsumedBy(netMocks[1].Object, It.IsAny<IEnumerable<int>[]>())).Returns(true);

            var nets = netMocks.Select(x => x.Object).ToArray();

            // Act
            var result = pruner.Prune(nets);

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(netMocks[0].Object, result[0]);
        }
    }
}