using System.Collections.Generic;

namespace InfraSim.Models
{
    public class TrafficRouting : ITrafficRouting
    {
        public List<IServer> Servers { get; private set; }

        public TrafficRouting()
        {
            Servers = new List<IServer>();
        }

        public TrafficRouting(List<IServer> servers)
        {
            Servers = servers ?? new List<IServer>();
        }

        public virtual void RouteTraffic(int requestCount)
        {
            int requests = CalculateRequests(requestCount);
            List<IServer> servers = ObtainServers();
            SendRequestsToServers(requests, servers);
        }

        public virtual int CalculateRequests(int requestCount)
        {
            return requestCount;
        }

        public virtual List<IServer> ObtainServers()
        {
            return Servers;
        }

        public virtual void SendRequestsToServers(int requestCount, List<IServer> servers)
        {
            if (servers.Count == 0)
                return;

            int requestsPerServer = requestCount / servers.Count;
            int remainingRequests = requestCount % servers.Count;

            for (int i = 0; i < servers.Count; i++)
            {
                int requests = requestsPerServer;
                if (i < remainingRequests)
                    requests++;

                servers[i].HandleRequests(requests);
            }
        }
    }
} 