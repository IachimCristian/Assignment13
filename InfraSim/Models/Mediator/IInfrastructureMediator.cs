using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public interface IInfrastructureMediator
    {
        ICluster Gateway { get; }
        ICluster Processors { get; }
        void AddServer(IServer server);
    }
} 