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
            RecalculateRequests();
        }

        public void RemoveServer(IServer server)
        {
            Servers.Remove(server);
            RecalculateRequests();
        }

        public override void HandleRequests(int requestsCount)
        {
            if (Servers.Count > 0)
            {
                int requestsPerServer = requestsCount / Servers.Count;
                int remainingRequests = requestsCount % Servers.Count;

                foreach (var server in Servers)
                {
                    int serverRequests = requestsPerServer;
                    if (remainingRequests > 0)
                    {
                        serverRequests++;
                        remainingRequests--;
                    }
                    server.HandleRequests(serverRequests);
                }
            }

            base.HandleRequests(requestsCount);
        }

        private void RecalculateRequests()
        {
            int totalRequests = 0;
            foreach (var server in Servers)
            {
                totalRequests += server.RequestsCount;
            }
            
            base.HandleRequests(totalRequests);
        }
    }
} 