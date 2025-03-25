namespace InfraSim.Models
{
    public class TemporaryStorageDecorator : ServerCapabilityDecorator
    {
        public TemporaryStorageDecorator(IServerCapability capability) : base(capability) { }

        public override long MaximumRequests => base.MaximumRequests * 100;
        public override int Cost => base.Cost + 1000;
    }
} 