namespace InfraSim.Models
{
    public class ServerCapability : IServerCapability
    {
        public long MaximumRequests { get; } = 1000;
        public int Cost { get; } = 2500;
    }
} 