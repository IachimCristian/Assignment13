using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public class CacheTrafficRouting : TrafficRouting
    {
        private readonly IEnumerable<IServer> _servers;

        public CacheTrafficRouting(IEnumerable<IServer> servers)
        {
            _servers = servers;
            Servers = servers.ToList();
        }

        public override void RouteTraffic(long requestCount)
        {
            List<IServer> servers = ObtainServers();
            SendRequestsToServers(CalculateRequests(requestCount), servers);
        }

        public override long CalculateRequests(long requestCount)
        {
            return (long)(requestCount * 0.8);
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == ServerType.Cache).ToList();
        }
    }
} 