using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public interface IServerList
    {
        List<IServer> Servers { get; }
        void AddServer(IServer server);
        void RemoveServer(IServer server);
    }
} 