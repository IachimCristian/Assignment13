namespace InfraSim.Models
{
    public interface IServerState
    {
        void Handle(IServer server);
    }
} 