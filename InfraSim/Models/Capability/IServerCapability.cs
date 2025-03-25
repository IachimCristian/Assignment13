namespace InfraSim.Models.Capability
{
    public interface IServerCapability
    {
        long MaximumRequests { get; }
        int Cost { get; }
    }
} 