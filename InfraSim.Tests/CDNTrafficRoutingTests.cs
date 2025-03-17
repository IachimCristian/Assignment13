using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class CDNTrafficRoutingTests
    {
        [Fact]
        public void CDNTrafficPercentage_ShouldReturn70Percent()
        {
            var routing = new CDNTrafficRouting();
            
            Assert.Equal(0.7, routing.CDNTrafficPercentage);
        }
        
        [Fact]
        public void RemainingTrafficPercentage_ShouldReturn30Percent()
        {
            var routing = new CDNTrafficRouting();
            
            Assert.Equal(0.3, routing.RemainingTrafficPercentage);
        }
        
        [Fact]
        public void SendRequestsToServers_WithCDNServers_ShouldRoute70PercentToCDNs()
        {
            var cdnServer = new Mock<IServer>();
            cdnServer.Setup(s => s.Type).Returns(ServerType.CDN);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var servers = new List<IServer> { cdnServer.Object, loadBalancer.Object };
            var routing = new CDNTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            cdnServer.Verify(s => s.HandleRequests(70), Times.Once);
            loadBalancer.Verify(s => s.HandleRequests(30), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithoutCDNServers_ShouldRouteAllTrafficToLoadBalancers()
        {
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { loadBalancer.Object, webServer.Object };
            var routing = new CDNTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            loadBalancer.Verify(s => s.HandleRequests(100), Times.Once);
            webServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void SendRequestsToServers_WithoutCDNsOrLoadBalancers_ShouldRouteAllTrafficToOtherServers()
        {
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { webServer.Object, dbServer.Object };
            var routing = new CDNTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            webServer.Verify(s => s.HandleRequests(50), Times.Once);
            dbServer.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithMultipleCDNs_ShouldDistributeEvenlyAmongCDNs()
        {
            var cdn1 = new Mock<IServer>();
            cdn1.Setup(s => s.Type).Returns(ServerType.CDN);
            
            var cdn2 = new Mock<IServer>();
            cdn2.Setup(s => s.Type).Returns(ServerType.CDN);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var servers = new List<IServer> { cdn1.Object, cdn2.Object, loadBalancer.Object };
            var routing = new CDNTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            cdn1.Verify(s => s.HandleRequests(35), Times.Once);
            cdn2.Verify(s => s.HandleRequests(35), Times.Once);
            loadBalancer.Verify(s => s.HandleRequests(30), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithEmptyServerList_ShouldNotThrowException()
        {
            var routing = new CDNTrafficRouting(new List<IServer>());
            
            var exception = Record.Exception(() => routing.RouteTraffic(100));
            Assert.Null(exception);
        }
    }
} 