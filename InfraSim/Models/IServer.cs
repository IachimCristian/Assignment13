namespace InfraSim.Models
{
    public interface IServer
    {
        ServerType Type { get; }

        IServerCapability Capability { get; }

        void HandleRequests(int requestsCount);
    }
} 