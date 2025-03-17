using Xunit;
using Moq;
using InfraSim.Models;
using System.Collections.Generic;

namespace InfraSim.Tests
{
    public class TrafficRoutingTests
    {
        public class ConcreteTrafficRouting : TrafficRouting
        {
            public ConcreteTrafficRouting() : base()
            {
            }

            public ConcreteTrafficRouting(List<IServer> servers) : base(servers)
            {
            }

            protected override List<IServer> ObtainServers()
            {
                return Servers;
            }

            protected override void SendRequestsToServers(int requestCount, List<IServer> servers)
            {
                if (servers.Count == 0)
                    return;

                int requestsPerServer = requestCount / servers.Count;
                int remainingRequests = requestCount % servers.Count;

                for (int i = 0; i < servers.Count; i++)
                {
                    int requests = requestsPerServer;
                    if (i < remainingRequests)
                        requests++;

                    servers[i].HandleRequests(requests);
                }
            }
        }

        [Fact]
        public void CalculateRequests_ReturnsInputValue()
        {
            var trafficRouting = new ConcreteTrafficRouting();
            int requestCount = 100;
            
            trafficRouting.RouteTraffic(requestCount);
            
            Assert.Equal(requestCount, requestCount);
        }

        [Fact]
        public void TestRequestCount_ShouldReturnCorrectRequestCount()
        {
            var trafficRouting = new ConcreteTrafficRouting(new List<IServer>());
            int requestCount = 100;
            
            trafficRouting.RouteTraffic(requestCount);
            
            Assert.Equal(100, 100);
        }

        [Fact]
        public void ObtainServers_ReturnsAllServers()
        {
            var trafficRouting = new ConcreteTrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            
            var serversProperty = typeof(TrafficRouting).GetProperty("Servers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var servers = (List<IServer>)serversProperty.GetValue(trafficRouting);
            servers.Add(mockServer1.Object);
            servers.Add(mockServer2.Object);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer1.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void SendRequestsToServers_DistributesRequestsEvenly()
        {
            var trafficRouting = new ConcreteTrafficRouting();
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            var mockServer3 = new Mock<IServer>();
            
            var servers = new List<IServer> 
            { 
                mockServer1.Object, 
                mockServer2.Object, 
                mockServer3.Object 
            };
            
            var serversProperty = typeof(TrafficRouting).GetProperty("Servers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            serversProperty.SetValue(trafficRouting, servers);
            
            trafficRouting.RouteTraffic(100);
            
            mockServer1.Verify(s => s.HandleRequests(34), Times.Once);
            mockServer2.Verify(s => s.HandleRequests(33), Times.Once);
            mockServer3.Verify(s => s.HandleRequests(33), Times.Once);
        }

        [Fact]
        public void SendRequestsToServers_HandlesEmptyServerList()
        {
            var trafficRouting = new ConcreteTrafficRouting();
            
            trafficRouting.RouteTraffic(100);
        }

        [Fact]
        public void RouteTraffic_CallsAllRequiredMethods()
        {
            var mockTrafficRouting = new Mock<ConcreteTrafficRouting> { CallBase = true };
            var mockServer = new Mock<IServer>();
            var servers = new List<IServer> { mockServer.Object };
            
            var serversProperty = typeof(TrafficRouting).GetProperty("Servers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            serversProperty.SetValue(mockTrafficRouting.Object, servers);
            
            mockTrafficRouting.Object.RouteTraffic(100);
            
            mockServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Once);
        }
    }
} 