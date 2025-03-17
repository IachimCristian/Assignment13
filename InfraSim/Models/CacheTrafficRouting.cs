using System.Collections.Generic;
using System.Linq;

namespace InfraSim.Models
{
    public class CacheTrafficRouting : TrafficRouting
    {
        private const double CACHE_TRAFFIC_PERCENTAGE = 0.5;
        private const double REMAINING_TRAFFIC_PERCENTAGE = 0.5;

        public CacheTrafficRouting() : base()
        {
        }

        public CacheTrafficRouting(List<IServer> servers) : base(servers)
        {
        }

        protected override List<IServer> ObtainServers()
        {
            var allServers = Servers;
            
            var cacheServers = allServers.Where(s => s.Type == ServerType.CacheServer).ToList();
            var loadBalancers = allServers.Where(s => s.Type == ServerType.LoadBalancer).ToList();
            var otherServers = allServers.Where(s => 
                s.Type != ServerType.CacheServer && 
                s.Type != ServerType.LoadBalancer).ToList();
            
            var result = new List<IServer>();
            result.AddRange(cacheServers);
            result.AddRange(loadBalancers);
            result.AddRange(otherServers);
            
            return result;
        }

        protected override void SendRequestsToServers(int requestCount, List<IServer> servers)
        {
            if (servers.Count == 0)
                return;

            var cacheServers = servers.Where(s => s.Type == ServerType.CacheServer).ToList();
            var loadBalancers = servers.Where(s => s.Type == ServerType.LoadBalancer).ToList();
            var otherServers = servers.Where(s => 
                s.Type != ServerType.CacheServer && 
                s.Type != ServerType.LoadBalancer).ToList();

            int cacheRequests = (int)(requestCount * CACHE_TRAFFIC_PERCENTAGE);
            int remainingRequests = requestCount - cacheRequests;

            if (cacheServers.Count > 0)
            {
                DistributeRequestsEvenly(cacheRequests, cacheServers);
            }
            else
            {
                remainingRequests += cacheRequests;
            }

            if (loadBalancers.Count > 0)
            {
                DistributeRequestsEvenly(remainingRequests, loadBalancers);
            }
            else if (otherServers.Count > 0)
            {
                DistributeRequestsEvenly(remainingRequests, otherServers);
            }
        }

        private void DistributeRequestsEvenly(int requestCount, List<IServer> servers)
        {
            if (requestCount <= 0 || servers.Count == 0)
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
        
        public double CacheTrafficPercentage => CACHE_TRAFFIC_PERCENTAGE;
        
        public double RemainingTrafficPercentage => REMAINING_TRAFFIC_PERCENTAGE;
    }
} 