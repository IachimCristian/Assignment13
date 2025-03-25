namespace InfraSim.Models.Capability
{
    public class TemporaryStorageDecorator : ServerCapabilityDecorator
    {
        public TemporaryStorageDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => Capability.MaximumRequests * 100;
        public override int Cost => Capability.Cost + 1000;
    }
} 