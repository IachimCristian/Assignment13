using Xunit;
using Moq;
using InfraSim.Models.Server;
using InfraSim.Models.Mediator;
using InfraSim.Models.Capability;

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

            var setupSequence = mockFactory.SetupSequence(f => f.CreateCluster());
            setupSequence.Returns(mockGateway.Object);
            setupSequence.Returns(mockProcessors.Object);

            var mediator = new InfrastructureMediator(mockFactory.Object, mockCommandManager.Object, mockDataMapper.Object);

            Assert.NotNull(mediator.Gateway);
            Assert.NotNull(mediator.Processors);
            mockFactory.Verify(f => f.CreateCluster(), Times.Exactly(2));
            mockGateway.Verify(g => g.AddServer(mockProcessors.Object), Times.Once);
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

            var setupSequence = mockFactory.SetupSequence(f => f.CreateCluster());
            setupSequence.Returns(mockGateway.Object);
            setupSequence.Returns(mockProcessors.Object);
            
            mockServer.Setup(s => s.ServerType).Returns(ServerType.CDN);

            var mediator = new InfrastructureMediator(mockFactory.Object, mockCommandManager.Object, mockDataMapper.Object);
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

            var setupSequence = mockFactory.SetupSequence(f => f.CreateCluster());
            setupSequence.Returns(mockGateway.Object);
            setupSequence.Returns(mockProcessors.Object);
            
            mockServer.Setup(s => s.ServerType).Returns(ServerType.LoadBalancer);

            var mediator = new InfrastructureMediator(mockFactory.Object, mockCommandManager.Object, mockDataMapper.Object);
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

            var setupSequence = mockFactory.SetupSequence(f => f.CreateCluster());
            setupSequence.Returns(mockGateway.Object);
            setupSequence.Returns(mockProcessors.Object);
            
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Cache);

            var mediator = new InfrastructureMediator(mockFactory.Object, mockCommandManager.Object, mockDataMapper.Object);
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

            var setupSequence = mockFactory.SetupSequence(f => f.CreateCluster());
            setupSequence.Returns(mockGateway.Object);
            setupSequence.Returns(mockProcessors.Object);
            
            mockServer.Setup(s => s.ServerType).Returns(ServerType.Server);

            var mediator = new InfrastructureMediator(mockFactory.Object, mockCommandManager.Object, mockDataMapper.Object);
            mediator.AddServer(mockServer.Object);

            mockCommandManager.Verify(cm => cm.Execute(It.IsAny<AddServerCommand>()), Times.Once);
            mockGateway.Verify(g => g.AddServer(mockServer.Object), Times.Never);
        }
    }
} 