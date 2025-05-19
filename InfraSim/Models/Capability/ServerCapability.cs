using InfraSim.Models.Server;

namespace InfraSim.Models.Capability
{
    public class ServerCapability : IServerCapability, ICapabilityFactory
    {
        public long MaximumRequests { get; } = 1000;
        public int Cost { get; } = 2500;

        public IServerCapability Create(ServerType type)
        {
            IServerCapability capability = new ServerCapability();

            switch (type)
            {
                case ServerType.Cache:
                    capability = new TemporaryStorageDecorator(capability);
                    break;

                case ServerType.LoadBalancer:
                    capability = new TrafficDistributionDecorator(capability);
                    break;

                case ServerType.CDN:
                    capability = new TemporaryStorageDecorator(capability);
                    capability = new TrafficDistributionDecorator(capability);
                    capability = new EdgeServerDecorator(capability);
                    break;
                    
                case ServerType.Database:
                    capability = new PersistentStorageDecorator(capability);
                    break;
            }

            return capability;
        }
    }
} 