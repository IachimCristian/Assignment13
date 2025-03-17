using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class CacheTrafficRoutingTests
    {
        [Fact]
        public void CacheTrafficPercentage_ShouldReturn50Percent()
        {
            var routing = new CacheTrafficRouting();
            
            Assert.Equal(0.5, routing.CacheTrafficPercentage);
        }
        
        [Fact]
        public void RemainingTrafficPercentage_ShouldReturn50Percent()
        {
            var routing = new CacheTrafficRouting();
            
            Assert.Equal(0.5, routing.RemainingTrafficPercentage);
        }
        
        [Fact]
        public void SendRequestsToServers_WithCacheServers_ShouldRoute50PercentToCaches()
        {
            var cacheServer = new Mock<IServer>();
            cacheServer.Setup(s => s.Type).Returns(ServerType.CacheServer);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var servers = new List<IServer> { cacheServer.Object, loadBalancer.Object };
            var routing = new CacheTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            cacheServer.Verify(s => s.HandleRequests(50), Times.Once);
            loadBalancer.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithoutCacheServers_ShouldRouteAllTrafficToLoadBalancers()
        {
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { loadBalancer.Object, webServer.Object };
            var routing = new CacheTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            loadBalancer.Verify(s => s.HandleRequests(100), Times.Once);
            webServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void SendRequestsToServers_WithoutCachesOrLoadBalancers_ShouldRouteAllTrafficToOtherServers()
        {
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { webServer.Object, dbServer.Object };
            var routing = new CacheTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            webServer.Verify(s => s.HandleRequests(50), Times.Once);
            dbServer.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithMultipleCaches_ShouldDistributeEvenlyAmongCaches()
        {
            var cache1 = new Mock<IServer>();
            cache1.Setup(s => s.Type).Returns(ServerType.CacheServer);
            
            var cache2 = new Mock<IServer>();
            cache2.Setup(s => s.Type).Returns(ServerType.CacheServer);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var servers = new List<IServer> { cache1.Object, cache2.Object, loadBalancer.Object };
            var routing = new CacheTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            cache1.Verify(s => s.HandleRequests(25), Times.Once);
            cache2.Verify(s => s.HandleRequests(25), Times.Once);
            loadBalancer.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithEmptyServerList_ShouldNotThrowException()
        {
            var routing = new CacheTrafficRouting(new List<IServer>());
            
            var exception = Record.Exception(() => routing.RouteTraffic(100));
            Assert.Null(exception);
        }
    }
} 