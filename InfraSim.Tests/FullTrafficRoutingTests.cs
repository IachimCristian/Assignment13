using Xunit;
using Moq;
using InfraSim.Models;
using InfraSim.Models.Server;
using InfraSim.Routing;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class FullTrafficRoutingTests
    {
        [Fact]
        public void CalculateRequests_ReturnsInputValue() // Test if the CalculateRequests method returns the input value
        {
            var servers = new List<IServer>(); // Create a list of servers 
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Create a new FullTrafficRouting 
            long requestCount = 100; 
            
            long result = trafficRouting.CalculateRequests(requestCount); 
            
            Assert.Equal(requestCount, result); // Assert that the result is equal to the request count 
        }

        [Fact]
        public void TestRequestCount_ShouldReturnCorrectRequestCount() // Test if the CalculateRequests method return the correct request count
        {
            var servers = new List<IServer>();
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Update constructor
            Assert.Equal(100L, trafficRouting.CalculateRequests(100)); // Added L suffix for long literal
        }

        [Fact]
        public void ObtainServers_ReturnsAllServers() // Test if the ObtainServers method returns all servers
        {
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            mockServer1.Setup(s => s.ServerType).Returns(ServerType.Server);
            mockServer2.Setup(s => s.ServerType).Returns(ServerType.Server);
            
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Update constructor
            
            var result = trafficRouting.ObtainServers();
            
            Assert.Equal(2, result.Count);
            Assert.Contains(mockServer1.Object, result);
            Assert.Contains(mockServer2.Object, result);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly() // Test if the SendRequestsToServers method distributes the requests evenly
        {
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
            
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Update constructor
            
            trafficRouting.SendRequestsToServers(100, servers);
            
            mockServer1.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer3.Verify(s => s.HandleRequests(33), Times.Once);
        }

        [Fact]
        public void SendRequestsToServers_HandlesEmptyServerList()
        {
            var servers = new List<IServer>();
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Update constructor
            
            trafficRouting.SendRequestsToServers(100, servers);
        }

        [Fact]
        public void RouteTraffic_CallsAllRequiredMethods()
        {
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Server);
            var servers = new List<IServer> { mockServer.Object };
            
            var trafficRouting = new FullTrafficRouting(servers, ServerType.Server); // Update constructor
            
            trafficRouting.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Once);
        }
    }
} 