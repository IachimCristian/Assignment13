using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator
    {
        public ICluster Gateway { get; private set; }
        public ICluster Processors { get; private set; }

        public InfrastructureMediator(IServerFactory serverFactory)
        {
            Gateway = (ICluster)serverFactory.CreateCluster();
            Processors = (ICluster)serverFactory.CreateCluster();
            Gateway.AddServer(Processors);
        }

        public void AddServer(IServer server)
        {
            switch (server.ServerType)
            {
                case ServerType.CDN:
                case ServerType.LoadBalancer:
                    Gateway.AddServer(server);
                    break;
                case ServerType.Cache:
                case ServerType.Server:
                    Processors.AddServer(server);
                    break;
            }
        }
    }
} 