using System;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private Guid _id = Guid.Empty;
        private ServerType _type = ServerType.Server;
        private IServerCapability _capability = new ServerCapability();
        private IServerState _state = new IdleState();

        public IServerBuilder WithId(Guid id)
        {
            _id = id;
            return this;
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
            if (_id != Guid.Empty)
                server.Id = _id;
            else
                server.Id = Guid.NewGuid();
            return server;
        }
    }
} 