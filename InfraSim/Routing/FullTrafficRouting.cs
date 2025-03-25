using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public class FullTrafficRouting : TrafficRouting
    {
        private readonly ServerType _serverType;

        public FullTrafficRouting(ServerType serverType)
        {
            _serverType = serverType;
        }

        public override int CalculateRequests(int requestCount)
        {
            return requestCount;
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == _serverType).ToList();
        }
    }
} 