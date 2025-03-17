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

        [Fact]
        public void Create_RegularServer_ReturnsBaseCapability()
        {
            var serverType = ServerType.WebServer;
            
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType<ServerCapability>(capability);
            Assert.Equal(1000, capability.MaximumRequests);
            Assert.Equal(2500, capability.Cost);
        }

        [Fact]
        public void Create_CacheServer_ReturnsTemporaryStorageCapability()
        {
            var serverType = ServerType.CacheServer;
            
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType<TemporaryStorageCapability>(capability);
            Assert.Equal(100, capability.MaximumRequests);
            Assert.Equal(3500, capability.Cost);
        }

        [Fact]
        public void Create_LoadBalancer_ReturnsTrafficDistributionCapability()
        {
            var serverType = ServerType.LoadBalancer;
            
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType<TrafficDistributionCapability>(capability);
            Assert.Equal(10000, capability.MaximumRequests);
            Assert.Equal(4000, capability.Cost);
        }

        [Fact]
        public void Create_CDN_ReturnsFullyDecoratedCapability()
        {
            var serverType = ServerType.CDN;
            
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType<EdgeServerCapability>(capability);
            Assert.Equal(1000, capability.MaximumRequests);
            Assert.Equal(55000, capability.Cost);
        }

        [Fact]
        public void Create_UnknownServerType_ReturnsBaseCapability()
        {
            var serverType = ServerType.DatabaseServer;
            
            var capability = _factory.Create(serverType);
            
            Assert.NotNull(capability);
            Assert.IsType<ServerCapability>(capability);
            Assert.Equal(1000, capability.MaximumRequests);
            Assert.Equal(2500, capability.Cost);
        }
    }
} 