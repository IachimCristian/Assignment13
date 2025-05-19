using InfraSim.Models.Server;

namespace InfraSim.Models.Capability
{
    public class PersistentStorageDecorator : ServerCapabilityDecorator
    {
        public PersistentStorageDecorator(IServerCapability decorated) : base(decorated) { }

        public override long MaximumRequests => Capability.MaximumRequests * 50;
        public override int Cost => Capability.Cost + 5000;
    }
} 