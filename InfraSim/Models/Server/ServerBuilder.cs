using System;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class ServerBuilder : IServerBuilder
    {
        private Guid _id;
        private ServerType _type;
        private IServerCapability _capability;
        private IServerState _state = new IdleState();
        private IValidatorStrategy _validator = new ServerValidator();

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

        public IServerBuilder WithValidator(IValidatorStrategy validator)
        {
            _validator = validator;
            return this;
        }

        public IServer Build()
        {
            IServer server;
            
            if (_type == ServerType.Cluster)
            {
                server = new Cluster(_capability, _validator);
            }
            else
            {
                server = new Server(_type, _capability, _validator);
            }
            
            server.Id = _id;
            server.State = _state;
            return server;
        }
    }
} 