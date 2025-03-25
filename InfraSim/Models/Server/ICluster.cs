using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public interface ICluster : IServer
    {
        void AddServer(IServer server);
        void RemoveServer(IServer server);
        List<IServer> Servers { get; }
    }
} 