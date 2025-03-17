namespace InfraSim.Models
{
    public class ServerCapabilityFactory : ICapabilityFactory
    {
        public IServerCapability Create(ServerType serverType)
        {
            var baseCapability = new ServerCapability();
            
            return serverType switch
            {
                ServerType.WebServer => baseCapability,
                
                ServerType.CacheServer => new TemporaryStorageCapability(baseCapability),
                
                ServerType.LoadBalancer => new TrafficDistributionCapability(baseCapability),
                
                ServerType.CDN => new EdgeServerCapability(
                    new TrafficDistributionCapability(
                        new TemporaryStorageCapability(baseCapability)
                    )
                ),
                
                _ => baseCapability
            };
        }
    }
} 