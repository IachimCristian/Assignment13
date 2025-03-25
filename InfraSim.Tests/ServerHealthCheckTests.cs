using Xunit;
using Moq;
using InfraSim.Models.Health;
using InfraSim.Models.Server;
using InfraSim.Models.Capability;

namespace InfraSim.Tests
{
    public class ServerHealthCheckTests
    {
        [Fact]
        public void IsIdle_ShouldReturnTrue_WhenServerRequestCountIsZero()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.RequestsCount).Returns(0);
            mockServer.Setup(s => s.ServerCapability).Returns(mockCapability.Object);
            
            var healthCheck = new ServerHealthCheck(mockServer.Object);
            
            Assert.True(healthCheck.IsIdle);
            Assert.False(healthCheck.IsNormal);
            Assert.False(healthCheck.IsOverloaded);
            Assert.False(healthCheck.IsFailed);
        }
        
        [Fact]
        public void IsNormal_ShouldReturnTrue_WhenServerLoadIsBetween1And79Percent()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.RequestsCount).Returns(500);
            mockServer.Setup(s => s.ServerCapability).Returns(mockCapability.Object);
            
            var healthCheck = new ServerHealthCheck(mockServer.Object);
            
            Assert.False(healthCheck.IsIdle);
            Assert.True(healthCheck.IsNormal);
            Assert.False(healthCheck.IsOverloaded);
            Assert.False(healthCheck.IsFailed);
        }
        
        [Fact]
        public void IsOverloaded_ShouldReturnTrue_WhenServerLoadIsBetween80And99Percent()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.RequestsCount).Returns(850);
            mockServer.Setup(s => s.ServerCapability).Returns(mockCapability.Object);
            
            var healthCheck = new ServerHealthCheck(mockServer.Object);
            
            Assert.False(healthCheck.IsIdle);
            Assert.False(healthCheck.IsNormal);
            Assert.True(healthCheck.IsOverloaded);
            Assert.False(healthCheck.IsFailed);
        }
        
        [Fact]
        public void IsFailed_ShouldReturnTrue_WhenServerLoadIs100PercentOrMore()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var mockServer = new Mock<IServer>();
            mockServer.Setup(s => s.RequestsCount).Returns(1000);
            mockServer.Setup(s => s.ServerCapability).Returns(mockCapability.Object);
            
            var healthCheck = new ServerHealthCheck(mockServer.Object);
            
            Assert.False(healthCheck.IsIdle);
            Assert.False(healthCheck.IsNormal);
            Assert.False(healthCheck.IsOverloaded);
            Assert.True(healthCheck.IsFailed);
        }
    }
} 