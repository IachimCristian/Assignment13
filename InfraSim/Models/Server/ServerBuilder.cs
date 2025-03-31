using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private ServerType _type;
        private IServerCapability _capability;
        private IServerState _state;

        public ServerBuilder()
        {
            Reset();
        }

        private void Reset()
        {
            _type = ServerType.Server;
            _capability = null;
            _state = new IdleState();
        }

        public IServerBuilder WithType(ServerType type)
        {
            _type = type;
            return this;
        }

        public IServerBuilder WithCapability(IServerCapability capability)
        {
            _capability = capability;
            return this;
        }

        public IServerBuilder WithState(IServerState state)
        {
            _state = state;
            return this;
        }

        public Server Build()
        {
            var server = new Server(_type, _capability);
            server.State = _state;
            Reset();
            return server;
        }
    }
} 