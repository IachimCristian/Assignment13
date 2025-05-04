using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator // For Gateway and Processors 
    {
        public ICluster Gateway { get; private set; }
        public ICluster Processors { get; private set; }
        private readonly ICommandManager _commandManager;
        private readonly IServerDataMapper _mapper;

        public InfrastructureMediator(IServerFactory serverFactory, ICommandManager commandManager, IServerDataMapper mapper) // For the InfrastructureMediator 
        {
            Gateway = serverFactory.CreateCluster();
            Processors = serverFactory.CreateCluster();
            Gateway.AddServer(Processors);
            _commandManager = commandManager;
            _mapper = mapper;
        }

        public void AddServer(IServer server) // For adding a server 
        {
            switch (server.ServerType)
            {
                case ServerType.CDN:
                case ServerType.LoadBalancer:
                    var addServerCommand = new AddServerCommand(Gateway, server, _mapper);
                    _commandManager.Execute(addServerCommand);
                    break;
                case ServerType.Cache:
                case ServerType.Server:
                    addServerCommand = new AddServerCommand(Processors, server, _mapper);
                    _commandManager.Execute(addServerCommand);
                    break;
            }
        }

        public IServerIterator Iterator => new ServerIterator(Gateway);

        public IServerIterator CreateServerIterator()
        {
            return new ServerIterator(Gateway);
        }
    }
} 