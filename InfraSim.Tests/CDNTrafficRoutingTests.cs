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
        public void CalculateRequests_Returns70Percent() // Test if the CalculateRequests method returns 70% of the requests 
        {
            var servers = new List<IServer>();
            var trafficRouting = new CDNTrafficRouting(servers);
            long requestCount = 100;
            
            long result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(70, result);
        }

        [Fact]
        public void ObtainServers_ReturnsCDNServers() // Test if the ObtainServers method returns CDN servers 
        {
            var mockCDN = new Mock<IServer>();
            var mockOther = new Mock<IServer>();
            
            mockCDN.Setup(s => s.ServerType).Returns(ServerType.CDN);
            mockOther.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            var servers = new List<IServer> { mockCDN.Object, mockOther.Object };
            var trafficRouting = new CDNTrafficRouting(servers);
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Single(result);
            Assert.Contains(mockCDN.Object, result);
            Assert.DoesNotContain(mockOther.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly() // Test if the SendRequestsToServers method distributes the requests evenly 
        {
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.CDN);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.CDN);
            
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            var trafficRouting = new CDNTrafficRouting(servers);
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(50), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(50), Times.Once);
        }

        [Fact]
        public void RouteTraffic_ProcessesRequests() // Test if the RouteTraffic method processes the requests 
        {
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.CDN);
            var servers = new List<IServer> { mockServer.Object };
            var trafficRouting = new CDNTrafficRouting(servers);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(70), Times.Once);
        }
    }
} 