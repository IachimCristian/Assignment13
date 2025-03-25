using Xunit;
using Moq;
using InfraSim.Models.Server;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Tests
{
    public class StateTransitionTests
    {
        [Fact] // Test if the RequestsCount property is set to zero and the state transitions to IdleState 
        public void RequestsCount_WhenSetToZero_ShouldTransitionToIdleState()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var server = new TestServer(ServerType.Server, mockCapability.Object);
            server.RequestsCount = 500;
            
            server.RequestsCount = 0;
            
            Assert.IsType<IdleState>(server.State);
        }
        
        [Fact] // Test if the RequestsCount property is set to 500 and the state transitions to NormalState 
        public void RequestsCount_WhenSetTo50Percent_ShouldTransitionToNormalState()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var server = new TestServer(ServerType.Server, mockCapability.Object);
            server.RequestsCount = 0;
            
            server.RequestsCount = 500; 
            
            Assert.IsType<NormalState>(server.State);
        }
        
        [Fact] // Test if the RequestsCount property is set to 850 and the state transitions to OverloadedState 
        public void RequestsCount_WhenSetTo85Percent_ShouldTransitionToOverloadedState()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var server = new TestServer(ServerType.Server, mockCapability.Object);
            server.RequestsCount = 500; 

            server.RequestsCount = 850; 
            
            Assert.IsType<OverloadedState>(server.State);
        }
        
        [Fact] // Test if the RequestsCount property is set to 1000 and the state transitions to FailedState 
        public void RequestsCount_WhenSetTo100Percent_ShouldTransitionToFailedState()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var server = new TestServer(ServerType.Server, mockCapability.Object);
            server.RequestsCount = 850;
            
            server.RequestsCount = 1000; 
   
            Assert.IsType<FailedState>(server.State);
        }
        
        [Fact]
        public void StateTransitions_ShouldFollowCorrectSequence()
        {
            var mockCapability = new Mock<IServerCapability>();
            mockCapability.Setup(c => c.MaximumRequests).Returns(1000);
            
            var server = new TestServer(ServerType.Server, mockCapability.Object);
            
            server.RequestsCount = 0;
            Assert.IsType<IdleState>(server.State);
            
            server.RequestsCount = 500;
            Assert.IsType<NormalState>(server.State);
            
            server.RequestsCount = 850;
            Assert.IsType<OverloadedState>(server.State);
            
            server.RequestsCount = 1000;
            Assert.IsType<FailedState>(server.State);
            
            server.RequestsCount = 850;
            Assert.IsType<OverloadedState>(server.State);
            
            server.RequestsCount = 500;
            Assert.IsType<NormalState>(server.State);
            
            server.RequestsCount = 0;
            Assert.IsType<IdleState>(server.State);
        }
    }
    
    public class TestServer : BaseServer
    {
        public TestServer(ServerType serverType, IServerCapability serverCapability) 
            : base(serverType, serverCapability)
        {
        }
    }
} 