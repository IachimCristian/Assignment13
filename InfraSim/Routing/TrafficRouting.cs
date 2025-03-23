using System.Collections.Generic;
using InfraSim.Models;

namespace InfraSim.Routing
{
    public abstract class TrafficRouting : ITrafficRouting
    {
        public List<IServer> Servers { get; set; }

        protected TrafficRouting()
        {
            Servers = new List<IServer>();
        }

        public void RouteTraffic(int requestCount)
        {
            int requests = CalculateRequests(requestCount);
            List<IServer> servers = ObtainServers();
            SendRequestsToServers(requests, servers);
        }

        public abstract int CalculateRequests(int requestCount);

        public abstract List<IServer> ObtainServers();

        public void SendRequestsToServers(int requestCount, List<IServer> servers)
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