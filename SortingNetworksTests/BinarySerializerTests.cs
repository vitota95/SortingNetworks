namespace SortingNetworksTests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SortingNetworks;
    using SortingNetworks.Utils;

    [TestClass]
    public class BinarySerializerTests
    {
        [TestMethod]
        public void Serialize_WithComparatorNetwork_CanBeDeserialized()
        {
            // Arrange
            var path = "serialized.dat";
            var net = new ComparatorNetwork(new[] { new Comparator(0, 2) });
            var sut = new BinarySerializer(path);

            // Act
            sut.Serialize(net);
            var result = sut.Deserialize<ComparatorNetwork>();

            // Arrange
            Assert.AreEqual(net.Comparators.Length, result.Comparators.Length);
            Assert.AreEqual(net.Comparators[0].X, result.Comparators[0].X);
            Assert.AreEqual(net.Comparators[0].Y, result.Comparators[0].Y);
        }
        
        [TestMethod]
        public void Serialize_WithNetworksList_CanBeDeserialized()
        {
            // Arrange
            var path = "serialized.dat";
            var net1 = new ComparatorNetwork(new[] { new Comparator(0, 2) });
            var net2 = new ComparatorNetwork(new[] { new Comparator(0, 2) });
            IReadOnlyList<IComparatorNetwork> nets = new List<IComparatorNetwork> { net1, net2 };
           
            var sut = new BinarySerializer(path);

            // Act
            sut.Serialize(nets);
            var result = sut.Deserialize<IReadOnlyList<IComparatorNetwork>>();

            // Arrange
            Assert.AreEqual(nets.Count, result.Count);
        }
    }
}
