namespace InfraSim.Models
{
    public class SecurityCapability : ServerCapabilityDecorator
    {
        public SecurityCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => (long)(Capability.MaximumRequests * 0.9);

        public override int Cost => Capability.Cost + 800;
    }
} 