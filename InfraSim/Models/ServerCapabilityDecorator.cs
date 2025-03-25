namespace InfraSim.Models
{
    public class ServerCapabilityDecorator : IServerCapability
    {
        protected IServerCapability Capability;

        public ServerCapabilityDecorator(IServerCapability capability)
        {
            Capability = capability;
        }

        public virtual long MaximumRequests { get => Capability.MaximumRequests; }
        public virtual int Cost { get => Capability.Cost; }
    }
} 