using Xunit;
using Moq;
using InfraSim.Models.Server;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Tests
{
    public class ServerBuilderTests // Test class for ServerBuilder class 
    {
        [Fact]
        public void Build_WithDefaultValues_CreatesServerWithCorrectDefaults() // Test method for Build method with default values 
        {            var mockCapability = new Mock<IServerCapability>();
            var builder = new ServerBuilder();

            var server = builder
                .WithCapability(mockCapability.Object)
                .Build();

            Assert.NotNull(server);
            Assert.Equal(ServerType.Server, server.ServerType);
            Assert.IsType<IdleState>(server.State);
            Assert.Same(mockCapability.Object, server.ServerCapability);
        }

        [Fact]
        public void Build_WithCustomValues_CreatesServerWithSpecifiedValues() // Test method for build method with custom values 
        {
            var mockCapability = new Mock<IServerCapability>();
            var mockState = new Mock<IServerState>();
            var builder = new ServerBuilder();

            var server = builder
                .WithType(ServerType.LoadBalancer)
                .WithCapability(mockCapability.Object)
                .WithState(mockState.Object)
                .Build();

            Assert.NotNull(server);
            Assert.Equal(ServerType.LoadBalancer, server.ServerType);
            Assert.Same(mockState.Object, server.State);
            Assert.Same(mockCapability.Object, server.ServerCapability);
        }

        [Fact]
        public void Build_SupportsMethodChaining() // Test method for build method with method chaining 
        {
            var mockCapability = new Mock<IServerCapability>();
            var builder = new ServerBuilder();

            var server = builder
                .WithType(ServerType.Cache)
                .WithCapability(mockCapability.Object)
                .WithState(new IdleState())
                .Build();

            Assert.NotNull(server);
        }
    }
} 