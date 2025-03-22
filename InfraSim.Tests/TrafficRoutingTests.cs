using Xunit;
using Moq;
using InfraSim.Models;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class TrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_ReturnsInputValue() // Test if the CalculateRequests method returns the input value
        {
            var trafficRouting = new TrafficRouting();
            int requestCount = 100;
            
            int result = trafficRouting.CalculateRequests(requestCount);
            
            Assert.Equal(requestCount, result);
        }

        [Fact]
        public void TestRequestCount_ShouldReturnCorrectRequestCount() // Test if the CalculateRequests method return the correct request count
        {
            TrafficRouting trafficRouting = new TrafficRouting(new List<IServer>());
            Assert.Equal(100, trafficRouting.CalculateRequests(100));
        }

        [Fact]
        public void ObtainServers_ReturnsAllServers() // Test if the ObtainServers method returns all servers
        {
            var trafficRouting = new TrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
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
            var trafficRouting = new TrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            var mockServer3 = new Mock<IServer>();
            
            var servers = new List<IServer> 
            { 
                mockServer1.Object, 
                mockServer2.Object, 
                mockServer3.Object 
            };
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(34), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer3.Verify(s => s.HandleRequests(33), Times.Once);
        }

        [Fact]
        public void SendRequestsToServers_HandlesEmptyServerList()
        {
            var trafficRouting = new TrafficRouting();
            var servers = new List<IServer>();
            
            trafficRouting.SendRequestsToServers(100, servers);
        }

        [Fact]
        public void RouteTraffic_CallsAllRequiredMethods()
        {
            var mockTrafficRouting = new Mock<TrafficRouting> { CallBase = true };
            var mockServer = new Mock<IServer>();
            var servers = new List<IServer> { mockServer.Object };
            
            mockTrafficRouting.Setup(tr => tr.CalculateRequests(It.IsAny<int>())).Returns(100);
            mockTrafficRouting.Setup(tr => tr.ObtainServers()).Returns(servers);
            
            mockTrafficRouting.Object.RouteTraffic(100);
            
            mockTrafficRouting.Verify(tr => tr.CalculateRequests(100), Times.Once);
            mockTrafficRouting.Verify(tr => tr.ObtainServers(), Times.Once);
            mockTrafficRouting.Verify(tr => tr.SendRequestsToServers(100, servers), Times.Once);
        }
    }
} 