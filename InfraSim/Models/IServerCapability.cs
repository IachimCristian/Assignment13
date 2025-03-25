namespace InfraSim.Models
{
    public interface IServerCapability
    {
        long MaximumRequests { get; }
        int Cost { get; }
    }
} 