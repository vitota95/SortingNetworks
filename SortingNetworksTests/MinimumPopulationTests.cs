using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace SortingNetworksTests
{
    [TestClass]
    public class MinimumPopulationTests
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
        public void MinimumPopulation9Inputs()
        {
            // Arrange
            var inputs = "9";
            var k = "25";
            var h = "25";

            // Act
            for (var i = int.Parse(h); i > 1; i--)
            {
                SortingNetworks.Program.Main(new[] { $"-s:{inputs}", $"-k:{k}", $"-k:{i}", "-t:12" });
                listenerMock.Verify(x => x.WriteLine($"1 Sorting Networks found with {inputs} inputs and {k} comparators"),  failMessage:$"{i} is the minimum");
            }
        }
    }
}
