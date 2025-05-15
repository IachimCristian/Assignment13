using Xunit;
using Moq;
using InfraSim.Models.Server;
using InfraSim.Models.Mediator;
using InfraSim.Models.Capability;
using System.Collections.Generic;
using System.Linq;

namespace InfraSim.Tests
{
    public class InfrastructureMediatorTests
    {
        [Fact]
        public void Constructor_InitializesGatewayAndProcessors()  // First we Initialize the Gateway and Processors 
        {
            var mockFactory = new Mock<IServerFactory>();
            var mockGateway = new Mock<ICluster>();
            var mockProcessors = new Mock<ICluster>();
            var mockCommandManager = new Mock<ICommandManager>();
            var mockDataMapper = new Mock<IServerDataMapper>();
            var mockCapabilityFactory = new Mock<ICapabilityFactory>();

            mockFactory.Setup(f => f.CreateGatewayCluster()).Returns(mockGateway.Object);
            mockFactory.Setup(f => f.CreateProcessorsCluster()).Returns(mockProcessors.Object);
            mockFactory.Setup(f => f.CreateCluster()).Returns(mockGateway.Object);
            
            mockDataMapper.Setup(m => m.GetAll()).Returns(new List<IServer>());

            var mediator = new InfrastructureMediator(
                mockCommandManager.Object, 
                mockDataMapper.Object, 
                mockCapabilityFactory.Object, 
                mockFactory.Object);

            Assert.NotNull(mediator.Gateway);
            Assert.NotNull(mediator.Processors);

            bool gatewayCreated = 
                mockFactory.Invocations.Any(i => i.Method.Name == "CreateGatewayCluster") ||
                mockFactory.Invocations.Any(i => i.Method.Name == "CreateCluster");
                
            bool processorsCreated = 
                mockFactory.Invocations.Any(i => i.Method.Name == "CreateProcessorsCluster") ||
                mockFactory.Invocations.Count(i => i.Method.Name == "CreateCluster") >= 2;
            
            Assert.True(gatewayCreated, "Gateway cluster was not created");
            Assert.True(processorsCreated, "Processors cluster was not created");
            
            mockGateway.Verify(g => g.AddServer(It.IsAny<IServer>()), Times.AtLeastOnce);
        }

        [Fact]
        public void AddServer_CDN_AddsToGateway() // In case we add CDN Server to Gateway 
        {
            var mockFactory = new Mock<IServerFactory>();
            var mockGateway = new Mock<ICluster>();
            var mockProcessors = new Mock<ICluster>();
            var mockServer = new Mock<IServer>();
            var mockCommandManager = new Mock<ICommandManager>();
            var mockDataMapper = new Mock<IServerDataMapper>();
            var mockCapabilityFactory = new Mock<ICapabilityFactory>();

            mockFactory.Setup(f => f.CreateGatewayCluster()).Returns(mockGateway.Object);
            mockFactory.Setup(f => f.CreateProcessorsCluster()).Returns(mockProcessors.Object);
            mockFactory.Setup(f => f.CreateCluster()).Returns(mockGateway.Object);
            
            mockDataMapper.Setup(m => m.GetAll()).Returns(new List<IServer>());
            mockServer.Setup(s => s.ServerType).Returns(ServerType.CDN);

            var mediator = new InfrastructureMediator(
                mockCommandManager.Object, 
                mockDataMapper.Object, 
                mockCapabilityFactory.Object, 
                mockFactory.Object);
            mediator.AddServer(mockServer.Object);

            mockCommandManager.Verify(cm => cm.Execute(It.IsAny<AddServerCommand>()), Times.Once);
            mockProcessors.Verify(p => p.AddServer(It.IsAny<IServer>()), Times.Never);
        }

        [Fact]
        public void AddServer_LoadBalancer_AddsToGateway() // In case we add LoadBalancer Server to Gateway 
        {
            var mockFactory = new Mock<IServerFactory>();
            var mockGateway = new Mock<ICluster>();
            var mockProcessors = new Mock<ICluster>();
            var mockServer = new Mock<IServer>();
            var mockCommandManager = new Mock<ICommandManager>();
            var mockDataMapper = new Mock<IServerDataMapper>();
            var mockCapabilityFactory = new Mock<ICapabilityFactory>();

            mockFactory.Setup(f => f.CreateGatewayCluster()).Returns(mockGateway.Object);
            mockFactory.Setup(f => f.CreateProcessorsCluster()).Returns(mockProcessors.Object);
            mockFactory.Setup(f => f.CreateCluster()).Returns(mockGateway.Object);
            
            mockDataMapper.Setup(m => m.GetAll()).Returns(new List<IServer>());
            mockServer.Setup(s => s.ServerType).Returns(ServerType.LoadBalancer);

            var mediator = new InfrastructureMediator(
                mockCommandManager.Object, 
                mockDataMapper.Object, 
                mockCapabilityFactory.Object, 
                mockFactory.Object);
            mediator.AddServer(mockServer.Object);

            mockCommandManager.Verify(cm => cm.Execute(It.IsAny<AddServerCommand>()), Times.Once);
            mockProcessors.Verify(p => p.AddServer(It.IsAny<IServer>()), Times.Never);
        }

        [Fact]
        public void AddServer_Cache_AddsToProcessors() // In case we add Cache Server to Processors 
        {
            var mockFactory = new Mock<IServerFactory>();
            var mockGateway = new Mock<ICluster>();
            var mockProcessors = new Mock<ICluster>();
            var mockServer = new Mock<IServer>();
            var mockCommandManager = new Mock<ICommandManager>();
            var mockDataMapper = new Mock<IServerDataMapper>();
            var mockCapabilityFactory = new Mock<ICapabilityFactory>();

            mockFactory.Setup(f => f.CreateGatewayCluster()).Returns(mockGateway.Object);
            mockFactory.Setup(f => f.CreateProcessorsCluster()).Returns(mockProcessors.Object);
            mockFactory.Setup(f => f.CreateCluster()).Returns(mockGateway.Object);
            
            mockDataMapper.Setup(m => m.GetAll()).Returns(new List<IServer>());
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Cache);

            var mediator = new InfrastructureMediator(
                mockCommandManager.Object, 
                mockDataMapper.Object, 
                mockCapabilityFactory.Object, 
                mockFactory.Object);
            mediator.AddServer(mockServer.Object);

            mockCommandManager.Verify(cm => cm.Execute(It.IsAny<AddServerCommand>()), Times.Once);
            mockGateway.Verify(g => g.AddServer(mockServer.Object), Times.Never);
        }

        [Fact]
        public void AddServer_Server_AddsToProcessors()
        {
            var mockFactory = new Mock<IServerFactory>();
            var mockGateway = new Mock<ICluster>();
            var mockProcessors = new Mock<ICluster>();
            var mockServer = new Mock<IServer>();
            var mockCommandManager = new Mock<ICommandManager>();
            var mockDataMapper = new Mock<IServerDataMapper>();
            var mockCapabilityFactory = new Mock<ICapabilityFactory>();

            mockFactory.Setup(f => f.CreateGatewayCluster()).Returns(mockGateway.Object);
            mockFactory.Setup(f => f.CreateProcessorsCluster()).Returns(mockProcessors.Object);
            mockFactory.Setup(f => f.CreateCluster()).Returns(mockGateway.Object);
            
            mockDataMapper.Setup(m => m.GetAll()).Returns(new List<IServer>());
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Server);

            var mediator = new InfrastructureMediator(
                mockCommandManager.Object, 
                mockDataMapper.Object, 
                mockCapabilityFactory.Object, 
                mockFactory.Object);
            mediator.AddServer(mockServer.Object);

            mockCommandManager.Verify(cm => cm.Execute(It.IsAny<AddServerCommand>()), Times.Once);
            mockGateway.Verify(g => g.AddServer(mockServer.Object), Times.Never);
        }
    }
} 