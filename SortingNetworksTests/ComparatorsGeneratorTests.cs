namespace SortingNetworksTests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SortingNetworks;

    [TestClass]
    public class ComparatorsGeneratorTests
    {
        [TestMethod]
        public void GenerateComparators_WithSize3_Generates3ExpectedComparators()
        {
            // Arrange
            var generator = new ComparatorsGenerator();
            var expectedComparators = new[] { new Comparator(0, 1), new Comparator(0, 2), new Comparator(1, 2), };

            // Act
            var result = generator.GenerateComparators(Enumerable.Range(0, 3).ToArray());

            // Assert
            Assert.AreEqual(3, result.Count);

            for (var i = 0; i < result.Count; i++)
            {
                var c = result[i];
                Assert.AreEqual(expectedComparators[i], c);
            }
        }
    }
}