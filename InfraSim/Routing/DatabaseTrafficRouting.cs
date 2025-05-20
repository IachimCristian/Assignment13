using System;
using System.Collections.Generic;
using System.Linq;
using InfraSim.Models;
using InfraSim.Models.Server;

namespace InfraSim.Routing
{
    public class DatabaseTrafficRouting : TrafficRouting
    {
        private readonly IEnumerable<IServer> _servers;

        public DatabaseTrafficRouting(IEnumerable<IServer> servers)
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
                    long calculatedRequests = CalculateRequests(requestCount);
                    SendRequestsToServers(calculatedRequests, servers);
                }
                
                PassToNext(requestCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error routing traffic to Database servers: {ex.Message}");
                
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
            return (long)(requestCount * 0.3);
        }

        public override List<IServer> ObtainServers()
        {
            if (Servers == null)
                return new List<IServer>();
                
            return Servers.Where(s => s != null && s.ServerType == ServerType.Database).ToList();
        }
    }
} 