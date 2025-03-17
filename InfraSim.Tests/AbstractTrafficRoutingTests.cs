using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class AbstractTrafficRoutingTests
    {
        private class TestTrafficRouting : AbstractTrafficRouting
        {
            public bool SendRequestsCalled { get; private set; }
            public int RequestCount { get; private set; }
            public List<IServer> ServersList { get; private set; } = new List<IServer>();

            public TestTrafficRouting() : base()
            {
            }

            public TestTrafficRouting(List<IServer> servers) : base(servers)
            {
            }

            protected override void SendRequestsToServers(int requestCount, List<IServer> servers)
            {
                SendRequestsCalled = true;
                RequestCount = requestCount;
                ServersList = servers;
            }
        }

        [Fact]
        public void RouteTraffic_CallsAllRequiredMethods()
        {
            var mockServer = new Mock<IServer>();
            var servers = new List<IServer> { mockServer.Object };
            var routing = new TestTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            Assert.True(routing.SendRequestsCalled);
            Assert.Equal(100, routing.RequestCount);
            Assert.Same(servers, routing.ServersList);
        }
        
        [Fact]
        public void CalculateRequests_ReturnsInputValue()
        {
            var routing = new TestTrafficRouting();
            int requestCount = 100;
            
            routing.RouteTraffic(requestCount);
            
            Assert.Equal(requestCount, routing.RequestCount);
        }
        
        [Fact]
        public void ObtainServers_ReturnsAllServers()
        {
            var mockServer1 = new Mock<IServer>();
            var mockServer2 = new Mock<IServer>();
            var servers = new List<IServer> { mockServer1.Object, mockServer2.Object };
            var routing = new TestTrafficRouting(servers);
            
            routing.RouteTraffic(100);
            
            Assert.Equal(2, routing.ServersList.Count);
            Assert.Contains(mockServer1.Object, routing.ServersList);
            Assert.Contains(mockServer2.Object, routing.ServersList);
        }
    }
} 