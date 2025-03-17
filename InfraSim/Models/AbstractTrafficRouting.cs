using System.Collections.Generic;

namespace InfraSim.Models
{
    public abstract class AbstractTrafficRouting : ITrafficRouting
    {
        protected List<IServer> Servers { get; private set; }

        protected AbstractTrafficRouting()
        {
            Servers = new List<IServer>();
        }

        protected AbstractTrafficRouting(List<IServer> servers)
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

        protected virtual List<IServer> ObtainServers()
        {
            return Servers;
        }

        protected abstract void SendRequestsToServers(int requestCount, List<IServer> servers);
    }
} 