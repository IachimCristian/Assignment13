using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class Server : BaseServer
    {
        public Server(ServerType serverType, IServerCapability serverCapability) 
            : base(serverType, serverCapability)
        {
        }

        public Server(ServerType serverType, IServerCapability capability, IValidatorStrategy validator) 
            : base(serverType, capability)
        {
            Validator = validator;
        }
    }
} 