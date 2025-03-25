using System.Collections.Generic;

namespace InfraSim.Models
{
    public class Cluster : BaseServer, ICluster
    {
        public List<IServer> Servers { get; private set; }

        public Cluster(IServerCapability serverCapability) 
            : base(ServerType.Cluster, serverCapability)
        {
            Servers = new List<IServer>();
        }

        public void AddServer(IServer server)
        {
            Servers.Add(server);
        }

        public void RemoveServer(IServer server)
        {
            Servers.Remove(server);
        }
    }
} 