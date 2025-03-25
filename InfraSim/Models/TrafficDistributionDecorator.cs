namespace InfraSim.Models
{
    public class TrafficDistributionDecorator : ServerCapabilityDecorator
    {
        public TrafficDistributionDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => Capability.MaximumRequests * 10000;
        public override int Cost => Capability.Cost + 1500;
    }
} 