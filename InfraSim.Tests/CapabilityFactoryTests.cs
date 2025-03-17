using System;
using Xunit;
using InfraSim.Models;

namespace InfraSim.Tests
{
    public class CapabilityFactoryTests
    {
        private readonly ICapabilityFactory _factory;

        public CapabilityFactoryTests()
        {
            _factory = new CapabilityFactory();
        }

        [Theory]
        [InlineData(ServerType.WebServer, typeof(ServerCapability), 1000, 2500)]
        [InlineData(ServerType.CacheServer, typeof(TemporaryStorageCapability), 100, 3500)]
        [InlineData(ServerType.LoadBalancer, typeof(TrafficDistributionCapability), 10000, 4000)]
        [InlineData(ServerType.CDN, typeof(EdgeServerCapability), 1000, 55000)]
        [InlineData(ServerType.DatabaseServer, typeof(ServerCapability), 1000, 2500)]
        public void Create_ServerType_ReturnsExpectedCapability(ServerType serverType, Type expectedType, long expectedRequests, int expectedCost)
        {
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType(expectedType, capability);
            Assert.Equal(expectedRequests, capability.MaximumRequests);
            Assert.Equal(expectedCost, capability.Cost);
        }
    }
} 