using InfraSim.Models.Server;

namespace InfraSim.Models.State
{
    public interface IServerState
    {
        void Handle(IServer server);
    }
} 