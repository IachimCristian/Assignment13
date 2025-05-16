using InfraSim.Models.Server;

namespace InfraSim.Models.Mediator
{
    public interface IInfrastructureMediator
    {
        ICluster Gateway { get; }
        ICluster Processors { get; }
        
        bool IsOK { get; }
        int TotalCost { get; }
        
        void AddServer(IServer server);
        IServerIterator Iterator { get; }
        IServerIterator CreateServerIterator();
    }
} 