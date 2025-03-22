using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;

namespace InfraSim.Routing
{
    public class FullTrafficRouting : TrafficRouting
    {
        private readonly ServerType _serverType;

        public FullTrafficRouting(ServerType serverType)
        {
            _serverType = serverType;
        }

        public override int CalculateRequests(int requestCount)
        {
            return requestCount;
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == _serverType).ToList();
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