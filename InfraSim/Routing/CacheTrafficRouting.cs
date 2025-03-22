using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;

namespace InfraSim.Routing
{
    public class CacheTrafficRouting : TrafficRouting
    {
        public override int CalculateRequests(int requestCount)
        {
            return (int)(requestCount * 0.8);
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == ServerType.Cache).ToList();
        }

        public override void SendRequestsToServers(int requestCount, List<IServer> servers)
        {
            if (servers.Count == 0) return;
            
            int requestsPerServer = requestCount / servers.Count;
            foreach (var server in servers)
            {
                server.HandleRequests(requestsPerServer);
            }
        }
    }
} 