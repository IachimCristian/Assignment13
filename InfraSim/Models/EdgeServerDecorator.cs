namespace InfraSim.Models
{
    public class EdgeServerDecorator : ServerCapabilityDecorator
    {
        public EdgeServerDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => base.MaximumRequests * 1000;
        public override int Cost => base.Cost + 50000;
    }
} 