namespace InfraSim.Models
{
    public class EdgeServerCapability : ServerCapabilityDecorator
    {
        public EdgeServerCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => 1000;

        public override int Cost => Capability.Cost + 50000;
    }
} 