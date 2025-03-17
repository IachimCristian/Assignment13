using System.Collections.Generic;
using InfraSim.Models;
using Moq;
using Xunit;

namespace InfraSim.Tests
{
    public class FullTrafficRoutingTests
    {
        [Fact]
        public void TargetServerType_ShouldReturnConstructorValue()
        {
            var routing = new FullTrafficRouting(ServerType.WebServer);
            
            Assert.Equal(ServerType.WebServer, routing.TargetServerType);
        }
        
        [Fact]
        public void SendRequestsToServers_WithTargetServers_ShouldRouteAllTrafficToTargetServers()
        {
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { webServer.Object, dbServer.Object };
            var routing = new FullTrafficRouting(ServerType.WebServer, servers);
            
            routing.RouteTraffic(100);
            
            webServer.Verify(s => s.HandleRequests(100), Times.Once);
            dbServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void SendRequestsToServers_WithoutTargetServers_ShouldRouteAllTrafficToOtherServers()
        {
            var loadBalancer = new Mock<IServer>();
            loadBalancer.Setup(s => s.Type).Returns(ServerType.LoadBalancer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { loadBalancer.Object, dbServer.Object };
            var routing = new FullTrafficRouting(ServerType.WebServer, servers);
            
            routing.RouteTraffic(100);
            
            loadBalancer.Verify(s => s.HandleRequests(50), Times.Once);
            dbServer.Verify(s => s.HandleRequests(50), Times.Once);
        }
        
        [Fact]
        public void SendRequestsToServers_WithMultipleTargetServers_ShouldDistributeEvenlyAmongTargetServers()
        {
            var web1 = new Mock<IServer>();
            web1.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var web2 = new Mock<IServer>();
            web2.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { web1.Object, web2.Object, dbServer.Object };
            var routing = new FullTrafficRouting(ServerType.WebServer, servers);
            
            routing.RouteTraffic(100);
            
            web1.Verify(s => s.HandleRequests(50), Times.Once);
            web2.Verify(s => s.HandleRequests(50), Times.Once);
            dbServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void SendRequestsToServers_WithEmptyServerList_ShouldNotThrowException()
        {
            var routing = new FullTrafficRouting(ServerType.WebServer, new List<IServer>());
            
            var exception = Record.Exception(() => routing.RouteTraffic(100));
            Assert.Null(exception);
        }
        
        [Fact]
        public void ObtainServers_ShouldPrioritizeTargetServers()
        {
            var webServer = new Mock<IServer>();
            webServer.Setup(s => s.Type).Returns(ServerType.WebServer);
            
            var dbServer = new Mock<IServer>();
            dbServer.Setup(s => s.Type).Returns(ServerType.DatabaseServer);
            
            var servers = new List<IServer> { dbServer.Object, webServer.Object };
            var routing = new FullTrafficRouting(ServerType.WebServer, servers);
            
            routing.RouteTraffic(100);
            
            webServer.Verify(s => s.HandleRequests(100), Times.Once);
            dbServer.Verify(s => s.HandleRequests(It.IsAny<int>()), Times.Never);
        }
    }
} 