using System.Collections.Generic;
using System.Linq;

namespace InfraSim.Models
{
    public class CDNTrafficRouting : AbstractTrafficRouting
    {
        private const double CDN_TRAFFIC_PERCENTAGE = 0.7; 
        private const double REMAINING_TRAFFIC_PERCENTAGE = 0.3; 


        public CDNTrafficRouting() : base()
        {
        }

        public CDNTrafficRouting(List<IServer> servers) : base(servers)
        {
        }

        protected override List<IServer> ObtainServers()
        {

            var allServers = base.ObtainServers();
            
            var cdnServers = allServers.Where(s => s.Type == ServerType.CDN).ToList();
            var loadBalancers = allServers.Where(s => s.Type == ServerType.LoadBalancer).ToList();
            var otherServers = allServers.Where(s => 
                s.Type != ServerType.CDN && 
                s.Type != ServerType.LoadBalancer).ToList();
            
            var result = new List<IServer>();
            result.AddRange(cdnServers);
            result.AddRange(loadBalancers);
            result.AddRange(otherServers);
            
            return result;
        }

        protected override void SendRequestsToServers(int requestCount, List<IServer> servers)
        {
            if (servers.Count == 0)
                return;

            var cdnServers = servers.Where(s => s.Type == ServerType.CDN).ToList();
            var loadBalancers = servers.Where(s => s.Type == ServerType.LoadBalancer).ToList();
            var otherServers = servers.Where(s => 
                s.Type != ServerType.CDN && 
                s.Type != ServerType.LoadBalancer).ToList();

            int cdnRequests = (int)(requestCount * CDN_TRAFFIC_PERCENTAGE);
            int remainingRequests = requestCount - cdnRequests;

            if (cdnServers.Count > 0)
            {
                DistributeRequestsEvenly(cdnRequests, cdnServers);
            }
            else
            {
                remainingRequests += cdnRequests;
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
        
        public double CDNTrafficPercentage => CDN_TRAFFIC_PERCENTAGE;
        
        public double RemainingTrafficPercentage => REMAINING_TRAFFIC_PERCENTAGE;
    }
} 