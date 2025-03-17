using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class CDNTrafficRoutingTests
    {
        [Fact]
        public void CDNTrafficPercentage_ShouldReturnCorrectValue() // Test to check if the CDN traffic Percentage is correct
        {
            var routing = new CDNTrafficRouting();
            
            Assert.Equal(0.7, routing.CDNTrafficPercentage);
        }
        
        [Fact]
        public void RemainingTrafficPercentage_ShouldReturnCorrectValue() // Test to check if Remaining Traffic Percentage is correct
        {
            var routing = new CDNTrafficRouting();
            
            Assert.Equal(0.3, routing.RemainingTrafficPercentage);
        }
        
        [Fact]
        public void ObtainServers_ShouldPrioritizeCDNServers() // Test to check is the CDN servers are prioritized 
        {
            var cdnServer = new Mock<IServer>();
            cdnServer.Setup(s => s.Type).Returns(ServerType.CDN);
            
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { webServer.Object, loadBalancer.Object, cdnServer.Object };
            var routing = new CDNTrafficRouting(servers);
            
            var obtainServersMethod = typeof(CDNTrafficRouting).GetMethod("ObtainServers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (List<IServer>)obtainServersMethod.Invoke(routing, null);
            
            Assert.Equal(cdnServer.Object, result[0]);
            Assert.Equal(loadBalancer.Object, result[1]);
            Assert.Equal(webServer.Object, result[2]);
        }
        
        [Fact]
        public void SendRequestsToServers_WithCDNServers_ShouldSend70PercentToCDN() // Test to check if 70% of the traffic is sent to the CDN servers 
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
        public void SendRequestsToServers_WithoutCDNServers_ShouldSendAllTrafficToLoadBalancers()
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
        public void SendRequestsToServers_WithoutCDNAndLoadBalancers_ShouldSendAllTrafficToOtherServers()
        {
            var webServer1 = new Mock<IServer>();
            webServer1.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var webServer2 = new Mock<IServer>();
            webServer2.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var servers = new List<IServer> { webServer1.Object, webServer2.Object };
            var routing = new CDNTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            webServer1.Verify(s => s.HandleRequests(50), Times.Once);
            webServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithMultipleCDNServers_ShouldDistributeEvenlyAmongCDNServers()
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
            var routing = new CDNTrafficRouting();
            
            var exception = Record.Exception(() => routing.RouteTraffic(100));
            Assert.Null(exception);
        }
        
        [Fact]
        public void CalculateRequests_ShouldReturnInputValue()
        {
            var routing = new CDNTrafficRouting();
            int requestCount = 100;
            
            var calculateRequestsMethod = typeof(TrafficRouting).GetMethod("CalculateRequests", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (int)calculateRequestsMethod.Invoke(routing, new object[] { requestCount });
            
            Assert.Equal(requestCount, result);
        }
    }
} 