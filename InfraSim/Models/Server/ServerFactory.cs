using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class ServerFactory : IServerFactory
    {
        private readonly ICapabilityFactory _capabilityFactory;
        private readonly ServerBuilder _builder;

        public ServerFactory(ICapabilityFactory capabilityFactory)
        {
            _capabilityFactory = capabilityFactory;
            _builder = new ServerBuilder();
        }

        private IServer CreateServerWithType(ServerType type)
        {
            var capability = _capabilityFactory.Create(type);
            return _builder
                .WithType(type)
                .WithCapability(capability)
                .Build();
        }

        public IServer CreateServer()
        {
            return CreateServerWithType(ServerType.Server);
        }

        public IServer CreateCache()
        {
            return CreateServerWithType(ServerType.Cache);
        }

        public IServer CreateLoadBalancer()
        {
            return CreateServerWithType(ServerType.LoadBalancer);
        }

        public IServer CreateCDN()
        {
            return CreateServerWithType(ServerType.CDN);
        }

        public IServer CreateCluster()
        {
            var capability = _capabilityFactory.Create(ServerType.Cluster);
            return new Cluster(capability);
        }
    }
} 