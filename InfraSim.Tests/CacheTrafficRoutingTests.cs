using Xunit;
using Moq;
using InfraSim.Models;
using InfraSim.Routing;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class CacheTrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_Returns80Percent()
        {
            var trafficRouting = new CacheTrafficRouting();
            int requestCount = 100;
            
            int result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(80, result);
        }

        [Fact]
        public void ObtainServers_ReturnsCacheServers()
        {
            var trafficRouting = new CacheTrafficRouting();
            var mockCache = new Mock<IServer>();
            var mockOther = new Mock<IServer>();
            
            mockCache.Setup(s => s.ServerType).Returns(ServerType.Cache);
            mockOther.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            trafficRouting.Servers.Add(mockCache.Object);
            trafficRouting.Servers.Add(mockOther.Object);
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Single(result);
            Assert.Contains(mockCache.Object, result);
            Assert.DoesNotContain(mockOther.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly()
        {
            var trafficRouting = new CacheTrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.Cache);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.Cache);
            
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(50), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }

        [Fact]
        public void RouteTraffic_ProcessesRequests()
        {
            var trafficRouting = new CacheTrafficRouting();
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Cache);
            trafficRouting.Servers.Add(mockServer.Object);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(80), Times.Once);
        }
    }
} 