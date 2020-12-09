namespace SortingNetworksTests
{
    using System.Diagnostics;

    using AutoFixture;
    using AutoFixture.AutoMoq;

    using Microsoft.VisualStudio.TestPlatform.TestHost;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ProgramTests
    {
        private Mock<TraceListener> listenerMock;

        [TestInitialize]
        public void Initialize()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            listenerMock = fixture.Create<Mock<TraceListener>>();
            SortingNetworks.Program.InitiateTracer(new[] { listenerMock.Object });
        }

        [TestMethod]
        public void Main_With3InputsAnd3Comparators()
        {
            // Arrange
            var inputs = "3";
            var k = "3";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}" });

            // Assert
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 1"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 2"), Times.Once);
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"), Times.Once);
        }
        
        [TestMethod]
        public void Main_With4InputsAnd5Comparators()
        {
            // Arrange
            var inputs = "4";
            var k = "5";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}" });

            // Assert
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 1"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 3"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 4"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 2"), Times.Once);
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"), Times.Once);
        }
        
        [TestMethod]
        public void Main_With5InputsAnd9Comparators()
        {
            // Arrange
            var inputs = "5";
            var k = "9";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}" });

            // Assert
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 1"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 3"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 6"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 11"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 10"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 7"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 4"), Times.Once);
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"));
        }
        
        [TestMethod]
        public void Main_With6InputsAnd12Comparators()
        {
            // Arrange
            var inputs = "6";
            var k = "12";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}"});

            // Assert
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 1"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 3"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 7"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 17"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 36"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 53"), Times.Exactly(2));
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 44"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 23"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 8"), Times.Once);
            listenerMock.Verify(x => x.WriteLine("Length after Prune: 4"), Times.Once);
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"));
        } 
        
        [TestMethod]
        public void Main_With6InputsAnd12Comparators_GivesCorrectResult()
        {
            // Arrange
            var inputs = "6";
            var k = "12";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}"});

            // Assert
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"));
        }
        
        [TestMethod]
        public void Main_With7InputsAnd16Comparators_GivesCorrectResult()
        {
            // Arrange
            var inputs = "7";
            var k = "16";

            // Act
            SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}"});

            // Assert
            listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"));
        }
    }
}
