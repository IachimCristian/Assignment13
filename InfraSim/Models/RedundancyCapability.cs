namespace InfraSim.Models
{
    public class RedundancyCapability : ServerCapabilityDecorator
    {
        public RedundancyCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => Capability.MaximumRequests * 2;

        public override int Cost => Capability.Cost + 1500;
    }
} 