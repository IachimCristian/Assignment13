using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public interface IServerList
    {
        List<IServer> Servers { get; set; }
        void AddServer(IServer server);
        void RemoveServer(IServer server);
    }
} 