namespace InfraSim.Models
{
    public class TemporaryStorageCapability : ServerCapabilityDecorator
    {
        public TemporaryStorageCapability(IServerCapability capability) : base(capability)
        {
        }

        public override long MaximumRequests => 100;

        public override int Cost => Capability.Cost + 1000;
    }
} 