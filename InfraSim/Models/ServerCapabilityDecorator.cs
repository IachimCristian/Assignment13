namespace InfraSim.Models
{
    public abstract class ServerCapabilityDecorator : IServerCapability
    {
        protected readonly IServerCapability Capability;

        protected ServerCapabilityDecorator(IServerCapability capability)
        {
            Capability = capability;
        }

        public virtual long MaximumRequests => Capability.MaximumRequests;

        public virtual int Cost => Capability.Cost;
    }
} 