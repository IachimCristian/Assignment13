using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class CacheTrafficRoutingTests
    {
        [Fact]
        public void CacheTrafficPercentage_ShouldReturnCorrectValue()
        {
            var routing = new CacheTrafficRouting();
            
            Assert.Equal(0.5, routing.CacheTrafficPercentage);
        }
        
        [Fact]
        public void RemainingTrafficPercentage_ShouldReturnCorrectValue()
        {
            var routing = new CacheTrafficRouting();
            
            Assert.Equal(0.5, routing.RemainingTrafficPercentage);
        }
        
        [Fact]
        public void ObtainServers_ShouldPrioritizeCacheServers()
        {
            var cacheServer = new Mock<IServer>();
            cacheServer.Setup(s => s.Type).Returns(ServerType.CacheServer);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { webServer.Object, loadBalancer.Object, cacheServer.Object };
            var routing = new CacheTrafficRouting(servers);
            
            var obtainServersMethod = typeof(CacheTrafficRouting).GetMethod("ObtainServers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (List<IServer>)obtainServersMethod.Invoke(routing, null);
            
            Assert.Equal(cacheServer.Object, result[0]);
            Assert.Equal(loadBalancer.Object, result[1]);
            Assert.Equal(webServer.Object, result[2]);
        }
        
        [Fact]
        public void SendRequestsToServers_WithCacheServers_ShouldSend50PercentToCache()
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
        public void SendRequestsToServers_WithoutCacheServers_ShouldSendAllTrafficToLoadBalancers()
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
        public void SendRequestsToServers_WithoutCacheAndLoadBalancers_ShouldSendAllTrafficToOtherServers()
        {
            var webServer1 = new Mock<IServer>();
            webServer1.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var webServer2 = new Mock<IServer>();
            webServer2.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { webServer1.Object, webServer2.Object };
            var routing = new CacheTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            webServer1.Verify(s => s.HandleRequests(50), Times.Once);
            webServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithMultipleCacheServers_ShouldDistributeEvenlyAmongCacheServers()
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
            var routing = new CacheTrafficRouting();
            
            var exception = Record.Exception(() => routing.RouteTraffic(100));
            Assert.Null(exception);
        }
        
        [Fact]
        public void CalculateRequests_ShouldReturnInputValue()
        {
            var routing = new CacheTrafficRouting();
            int requestCount = 100;
            
            var calculateRequestsMethod = typeof(TrafficRouting).GetMethod("CalculateRequests", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)calculateRequestsMethod.Invoke(routing, new object[] { requestCount });
            
            Assert.Equal(requestCount, result);
        }
    }
} 