using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public interface IInfrastructureMediator
    {
        IServer Gateway { get; }
        IServer Processors { get; }
        void AddServer(IServer server);
    }
} 