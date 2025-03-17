using System.Collections.Generic;

namespace InfraSim.Models
{
    public abstract class TrafficRouting : ITrafficRouting
    {
        protected List<IServer> Servers { get; private set; }

        protected TrafficRouting()
        {
            Servers = new List<IServer>();
        }

        protected TrafficRouting(List<IServer> servers)
        {
            Servers = servers ?? new List<IServer>();
        }

        public void RouteTraffic(int requestCount)
        {
            int requests = CalculateRequests(requestCount);
            List<IServer> servers = ObtainServers();
            SendRequestsToServers(requests, servers);
        }

        protected virtual int CalculateRequests(int requestCount)
        {
            return requestCount;
        }

        protected abstract List<IServer> ObtainServers();

        protected abstract void SendRequestsToServers(int requestCount, List<IServer> servers);
    }
} 