using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator // For Gateway and Processors 
    {
        public ICluster Gateway { get; private set; }
        public ICluster Processors { get; private set; }

        public InfrastructureMediator(IServerFactory serverFactory) // For the InfrastructureMediator 
        {
            Gateway = serverFactory.CreateCluster();
            Processors = serverFactory.CreateCluster();
            Gateway.AddServer(Processors);
        }

        public void AddServer(IServer server) // For adding a server 
        {
            switch (server.ServerType)
            {
                case ServerType.CDN: // In case the server is CDN 
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