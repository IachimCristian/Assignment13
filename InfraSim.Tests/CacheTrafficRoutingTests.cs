using Xunit;
using Moq;
using InfraSim.Models;
using InfraSim.Models.Server;
using InfraSim.Routing;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class CacheTrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_Returns80Percent()
        {
            var servers = new List<IServer>();
            var trafficRouting = new CacheTrafficRouting(servers);
            long requestCount = 100;
            
            long result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(80, result);
        }

        [Fact]
        public void ObtainServers_ReturnsCacheServers()
        {
            var mockCache = new Mock<IServer>();
            var mockOther = new Mock<IServer>();
            
            mockCache.Setup(s => s.ServerType).Returns(ServerType.Cache);
            mockOther.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            var servers = new List<IServer> { mockCache.Object, mockOther.Object };
            var trafficRouting = new CacheTrafficRouting(servers);
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Single(result);
            Assert.Contains(mockCache.Object, result);
            Assert.DoesNotContain(mockOther.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly()
        {
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.Cache);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.Cache);
            
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            var trafficRouting = new CacheTrafficRouting(servers);
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(50), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }

        [Fact]
        public void RouteTraffic_ProcessesRequests()
        {
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Cache);
            var servers = new List<IServer> { mockServer.Object };
            var trafficRouting = new CacheTrafficRouting(servers);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(80), Times.Once);
        }
    }
} 