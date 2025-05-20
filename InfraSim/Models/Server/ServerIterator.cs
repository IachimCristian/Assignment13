using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public class ServerIterator : IServerIterator
    {
        private readonly List<IServer> _servers;
        public int Position { get; private set; } = 0;

        public ServerIterator(ICluster cluster)
        {
            if (cluster == null)
            {
                _servers = new List<IServer>();
                return;
            }
            
            _servers = GetServers(cluster);
        }

        private List<IServer> GetServers(ICluster cluster)
        {
            var servers = new List<IServer>();
            
            try
            {
                if (cluster == null || cluster.Servers == null)
                    return servers;
                    
                foreach (var server in cluster.Servers)
                {
                    try
                    {
                        if (server == null)
                            continue;
                            
                        if (server is ICluster subCluster)
                        {
                            servers.AddRange(GetServers(subCluster));
                        }
                        else
                        {
                            servers.Add(server);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"Error processing server in iterator: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error getting servers from cluster: {ex.Message}");
            }
            
            return servers;
        }

        public bool HasNext => Position < (_servers?.Count ?? 0);

        public IServer Next 
        { 
            get 
            {
                try
                {
                    return HasNext ? _servers[Position++] : null;
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Error getting next server: {ex.Message}");
                    Position++; 
                    return null;
                }
            }
        }
    }
} 