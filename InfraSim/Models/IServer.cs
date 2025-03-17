namespace InfraSim.Models
{
    public interface IServer
    {
        ServerType Type { get; }

        void HandleRequests(int requestsCount);
    }
} 