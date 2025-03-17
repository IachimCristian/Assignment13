namespace InfraSim.Models
{
    public class TrafficDistributionCapability : ServerCapabilityDecorator
    {
        public TrafficDistributionCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => 10000;

        public override int Cost => Capability.Cost + 1500;
    }
} 