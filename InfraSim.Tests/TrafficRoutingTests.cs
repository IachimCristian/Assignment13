using Xunit;
using Moq;
using InfraSim.Models;
using InfraSim.Routing;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class TrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_ReturnsInputValue() // Test if the CalculateRequests method returns the input value
        {
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            int requestCount = 100;
            
            int result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(requestCount, result);
        }

        [Fact]
        public void TestRequestCount_ShouldReturnCorrectRequestCount() // Test if the CalculateRequests method return the correct request count
        {
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            Assert.Equal(100, trafficRouting.CalculateRequests(100));
        }

        [Fact]
        public void ObtainServers_ReturnsAllServers() // Test if the ObtainServers method returns all servers
        {
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.Server);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            trafficRouting.Servers.Add(mockServer1.Object);
            trafficRouting.Servers.Add(mockServer2.Object);
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Equal(2, result.Count);
            Assert.Contains(mockServer1.Object, result);
            Assert.Contains(mockServer2.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly() // Test if the SendRequestsToServers method distributes the requests evenly
        {
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            var mockServer3 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.Server);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.Server);
            mockServer3.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            var servers = new List<IServer> 
            { 
                mockServer1.Object, 
                mockServer2.Object, 
                mockServer3.Object 
            };
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer3.Verify(s => s.HandleRequests(33), Times.Once);
        }

        [Fact]
        public void SendRequestsToServers_HandlesEmptyServerList()
        {
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            var servers = new List<IServer>();
            
            trafficRouting.SendRequestsToServers(100, servers);
        }

        [Fact]
        public void RouteTraffic_CallsAllRequiredMethods()
        {
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Server);
            var servers = new List<IServer> { mockServer.Object };
            
            var trafficRouting = new FullTrafficRouting(ServerType.Server);
            trafficRouting.Servers = servers;
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Once);
        }
    }
} 