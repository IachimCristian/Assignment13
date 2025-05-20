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
            try
            {
                List<IServer> servers = ObtainServers();
                if (servers != null && servers.Count > 0)
                {
                    SendRequestsToServers(CalculateRequests(requestCount), servers);
                }
                
                PassToNext(requestCount);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error routing traffic to {_serverType} servers: {ex.Message}");
                
                try
                {
                    if (NextHandler != null)
                    {
                        NextHandler.DeliverRequests(requestCount);
                    }
                }
                catch
                {
                }
            }
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