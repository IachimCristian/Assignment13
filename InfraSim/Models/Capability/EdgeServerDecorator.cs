namespace InfraSim.Models.Capability
{
    public class EdgeServerDecorator : ServerCapabilityDecorator
    {
        public EdgeServerDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => Capability.MaximumRequests * 1000;
        public override int Cost => Capability.Cost + 50000;
    }
} 