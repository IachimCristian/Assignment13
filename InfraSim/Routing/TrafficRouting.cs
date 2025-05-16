using System.Collections.Generic;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public abstract class TrafficRouting : TrafficDelivery, ITrafficRouting
    {
        public List<IServer> Servers { get; set; }

        protected TrafficRouting()
        {
            Servers = new List<IServer>();
        }

        public abstract void RouteTraffic(long requestCount);
        
        public abstract long CalculateRequests(long requestCount);

        public abstract List<IServer> ObtainServers();

        public void SendRequestsToServers(long requestCount, List<IServer> servers)
        {
            if (servers.Count == 0) return;
            
            long requestsPerServer = requestCount / servers.Count;
            foreach (var server in servers)
            {
                server.HandleRequests((int)requestsPerServer);
            }
        }
        
        public override void DeliverRequests(long requestCount)
        {
            RouteTraffic(requestCount);
            long remainingRequests = requestCount - CalculateRequests(requestCount);
            if (remainingRequests > 0)
            {
                NextHandler?.DeliverRequests(remainingRequests);
            }
            else
            {
                NextHandler?.DeliverRequests(requestCount);
            }
        }
    }
} 