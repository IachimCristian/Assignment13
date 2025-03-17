using System.Collections.Generic;
using System.Linq;

namespace InfraSim.Models
{
    public class FullTrafficRouting : TrafficRouting
    {
        private readonly ServerType _targetServerType;

        public FullTrafficRouting(ServerType targetServerType) : base()
        {
            _targetServerType = targetServerType;
        }

        public FullTrafficRouting(ServerType targetServerType, List<IServer> servers) : base(servers)
        {
            _targetServerType = targetServerType;
        }

        public ServerType TargetServerType => _targetServerType;

        protected override List<IServer> ObtainServers()
        {
            var allServers = Servers;
            
            var targetServers = allServers.Where(s => s.Type == _targetServerType).ToList();
            var otherServers = allServers.Where(s => s.Type != _targetServerType).ToList();
            
            var result = new List<IServer>();
            result.AddRange(targetServers);
            result.AddRange(otherServers);
            
            return result;
        }

        protected override void SendRequestsToServers(int requestCount, List<IServer> servers)
        {
            if (servers.Count == 0)
                return;

            var targetServers = servers.Where(s => s.Type == _targetServerType).ToList();
            var otherServers = servers.Where(s => s.Type != _targetServerType).ToList();

            if (targetServers.Count > 0)
            {
                DistributeRequestsEvenly(requestCount, targetServers);
            }
            else if (otherServers.Count > 0)
            {
                DistributeRequestsEvenly(requestCount, otherServers);
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
    }
} 