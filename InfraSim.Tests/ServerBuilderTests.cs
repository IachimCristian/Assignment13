using Xunit;
using Moq;
using InfraSim.Models.Server;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Tests
{
    public class ServerBuilderTests
    {
        [Fact]
        public void Build_WithDefaultValues_CreatesServerWithCorrectDefaults()
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
        public void Build_WithCustomValues_CreatesServerWithSpecifiedValues()
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
        public void Build_SupportsMethodChaining()
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