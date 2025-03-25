using Xunit;
using InfraSim.Models;

namespace InfraSim.Tests
{
    public class ServerCapabilityTests
    {
        [Theory]
        [InlineData(ServerType.Server, 1000, 2500)] // Base case 
        [InlineData(ServerType.Cache, 100000, 3500)] // Cache has a temporary storage decorator
        [InlineData(ServerType.LoadBalancer, 10000000, 4000)] // Load balancer has a traffic distribution decorator 
        [InlineData(ServerType.CDN, 1000000000000, 55000)] // CDN has a temporary storage decorator  
        public void Create_ReturnsCorrectCapability(ServerType type, long expectedRequests, int expectedCost)
        {
            var factory = new ServerCapability();

            var capability = factory.Create(type);

            Assert.Equal(expectedRequests, capability.MaximumRequests);
            Assert.Equal(expectedCost, capability.Cost);
        }
    }
} 