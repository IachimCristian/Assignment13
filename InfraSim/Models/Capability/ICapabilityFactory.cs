using InfraSim.Models.Server;

namespace InfraSim.Models.Capability
{
    public interface ICapabilityFactory
    {
        IServerCapability Create(ServerType type);
    }
} 