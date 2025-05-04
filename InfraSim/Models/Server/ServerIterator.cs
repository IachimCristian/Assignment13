using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public class ServerIterator : IServerIterator
    {
        private readonly List<IServer> _servers;
        public int Position { get; private set; } = 0;

        public ServerIterator(ICluster cluster)
        {
            _servers = GetServers(cluster);
        }

        private List<IServer> GetServers(ICluster cluster)
        {
            var servers = new List<IServer>();
            foreach (var server in cluster.Servers)
            {
                if (server is ICluster subCluster)
                {
                    servers.AddRange(GetServers(subCluster));
                }
                else
                {
                    servers.Add(server);
                }
            }
            return servers;
        }

        public bool HasNext => Position < _servers.Count;

        public IServer Next => HasNext ? _servers[Position++] : null;
    }
} 