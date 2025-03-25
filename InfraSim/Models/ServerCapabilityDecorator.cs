namespace InfraSim.Models
{
    public abstract class ServerCapabilityDecorator : IServerCapability
    {
        protected IServerCapability Capability;

        protected ServerCapabilityDecorator(IServerCapability capability)
        {
            Capability = capability;
        }

        public abstract long MaximumRequests { get; }
        public abstract int Cost { get; }
    }
} 