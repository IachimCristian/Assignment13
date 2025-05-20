using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public class CDNTrafficRouting : TrafficRouting
    {
        private readonly IEnumerable<IServer> _servers;

        public CDNTrafficRouting(IEnumerable<IServer> servers)
        {
            _servers = servers;
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
                System.Console.WriteLine($"Error routing traffic to CDN servers: {ex.Message}");
                
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
            return (long)(requestCount * 0.7);
        }

        public override List<IServer> ObtainServers()
        {
            return Servers.Where(s => s.ServerType == ServerType.CDN).ToList();
        }
    }
} 