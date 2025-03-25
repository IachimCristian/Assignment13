using Xunit;
using Moq;
using InfraSim.Models;
using InfraSim.Models.Server;
using InfraSim.Routing;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class CDNTrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_Returns70Percent()
        {
            var trafficRouting = new CDNTrafficRouting();
            int requestCount = 100;
            
            int result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(70, result);
        }

        [Fact]
        public void ObtainServers_ReturnsCDNServers()
        {
            var trafficRouting = new CDNTrafficRouting();
            var mockCDN = new Mock<IServer>();
            var mockOther = new Mock<IServer>();
            
            mockCDN.Setup(s => s.ServerType).Returns(ServerType.CDN);
            mockOther.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            trafficRouting.Servers.Add(mockCDN.Object);
            trafficRouting.Servers.Add(mockOther.Object);
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Single(result);
            Assert.Contains(mockCDN.Object, result);
            Assert.DoesNotContain(mockOther.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly()
        {
            var trafficRouting = new CDNTrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.CDN);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.CDN);
            
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(50), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }

        [Fact]
        public void RouteTraffic_ProcessesRequests()
        {
            var trafficRouting = new CDNTrafficRouting();
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.CDN);
            trafficRouting.Servers.Add(mockServer.Object);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(70), Times.Once);
        }
    }
} 