namespace InfraSim.Models
{
    public interface ICapabilityFactory
    {
        IServerCapability Create(ServerType serverType);
    }
} 