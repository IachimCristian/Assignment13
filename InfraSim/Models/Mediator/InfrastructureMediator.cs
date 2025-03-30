using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator
    {
        public IServer Gateway { get; private set; }
        public IServer Processors { get; private set; }

        public InfrastructureMediator(IServerFactory serverFactory)
        {
            Gateway = serverFactory.CreateCluster();
            Processors = serverFactory.CreateCluster();
            ((ICluster)Gateway).AddServer(Processors);
        }

        public void AddServer(IServer server)
        {
            switch (server.ServerType)
            {
                case ServerType.CDN:
                case ServerType.LoadBalancer:
                    ((ICluster)Gateway).AddServer(server);
                    break;
                case ServerType.Cache:
                case ServerType.Server:
                    ((ICluster)Processors).AddServer(server);
                    break;
            }
        }
    }
} 