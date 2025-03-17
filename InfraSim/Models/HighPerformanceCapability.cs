namespace InfraSim.Models
{
    public class HighPerformanceCapability : ServerCapabilityDecorator
    {
        public HighPerformanceCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => (long)(Capability.MaximumRequests * 1.5);

        public override int Cost => Capability.Cost + 1000;
    }
} 