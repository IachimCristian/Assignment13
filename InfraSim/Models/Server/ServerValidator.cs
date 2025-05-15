using InfraSim.Models.State;

namespace InfraSim.Models.Server
{
    public class ServerValidator : IValidatorStrategy
    {
        public bool Validate(IServer server)
        {
            return !(server.State is FailedState);
        }
    }
} 