namespace InfraSim.Models
{
    public class CapabilityFactory : ICapabilityFactory
    {
        public IServerCapability Create(ServerType serverType)
        {
            return serverType switch
            {
                ServerType.CacheServer => CreateCacheServerCapability(),
                ServerType.LoadBalancer => CreateLoadBalancerCapability(),
                ServerType.CDN => CreateCDNCapability(),
                _ => CreateBaseCapability()
            };
        }

        private IServerCapability CreateBaseCapability()
        {
            return new ServerCapability();
        }

        private IServerCapability CreateCacheServerCapability()
        {
            var baseCapability = CreateBaseCapability();
            return new TemporaryStorageCapability(baseCapability);
        }

        private IServerCapability CreateLoadBalancerCapability()
        {
            var baseCapability = CreateBaseCapability();
            return new TrafficDistributionCapability(baseCapability);
        }

        private IServerCapability CreateCDNCapability()
        {
            var baseCapability = CreateBaseCapability();
            var withStorage = new TemporaryStorageCapability(baseCapability);
            var withTraffic = new TrafficDistributionCapability(withStorage);
            return new EdgeServerCapability(withTraffic);
        }
    }
} 