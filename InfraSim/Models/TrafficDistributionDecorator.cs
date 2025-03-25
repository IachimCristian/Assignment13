namespace InfraSim.Models
{
    public class TrafficDistributionDecorator : ServerCapabilityDecorator
    {
        public TrafficDistributionDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => base.MaximumRequests * 10000;
        public override int Cost => base.Cost + 1500;
    }
} 