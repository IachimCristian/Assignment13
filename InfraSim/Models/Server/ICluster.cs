using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public interface ICluster : IServer, IServerList
    {
        new List<IServer> Servers { get; set; }
        new void AddServer(IServer server);
        new void RemoveServer(IServer server);
    }
} 