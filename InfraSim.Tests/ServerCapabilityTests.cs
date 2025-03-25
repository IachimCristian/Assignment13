using Xunit;
using InfraSim.Models;

namespace InfraSim.Tests
{
    public class ServerCapabilityTests
    {
        [Theory]
        [InlineData(ServerType.Server, 1000, 2500)]
        [InlineData(ServerType.Cache, 100000, 3500)]
        [InlineData(ServerType.LoadBalancer, 10000000, 4000)]
        [InlineData(ServerType.CDN, 1000000000000, 55000)]
        public void Create_ReturnsCorrectCapability(ServerType type, long expectedRequests, int expectedCost)
        {
            var factory = new ServerCapability();

            var capability = factory.Create(type);

            Assert.Equal(expectedRequests, capability.MaximumRequests);
            Assert.Equal(expectedCost, capability.Cost);
        }
    }
} 