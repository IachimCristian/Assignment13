using InfraSim.Models.Server;

namespace InfraSim.Models.Server
{
    public interface IValidatorStrategy
    {
        bool Validate(IServer server);
    }
} 