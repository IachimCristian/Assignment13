using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public class FullTrafficRouting : TrafficRouting
    {
        private readonly IEnumerable<IServer> _servers;
        private readonly ServerType _serverType;

        public FullTrafficRouting(IEnumerable<IServer> servers, ServerType serverType)
        {
            _servers = servers;
            _serverType = serverType;
            Servers = servers.ToList();
        }

        public override void RouteTraffic(long requestCount)
        {
            List<IServer> servers = ObtainServers();
            SendRequestsToServers(CalculateRequests(requestCount), servers);
        }

        public override long CalculateRequests(long requestCount)
        {
            return requestCount;
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == _serverType).ToList();
        }
    }
} 