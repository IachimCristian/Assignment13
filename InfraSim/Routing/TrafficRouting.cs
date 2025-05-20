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
            try
            {
                if (servers == null || servers.Count == 0) return;
                
                long requestsPerServer = requestCount / servers.Count;
                foreach (var server in servers)
                {
                    try
                    {
                        if (server != null)
                        {
                            server.HandleRequests((int)requestsPerServer);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine($"Error handling requests for server: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error sending requests to servers: {ex.Message}");
            }
        }
        
        public override void DeliverRequests(long requestCount)
        {
            try
            {
                RouteTraffic(requestCount);
                
                try
                {
                    long remainingRequests = requestCount - CalculateRequests(requestCount);
                    if (remainingRequests > 0)
                    {
                        PassToNext(remainingRequests);
                    }
                    else
                    {
                        PassToNext(requestCount);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Error calculating or passing remaining requests: {ex.Message}");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error in DeliverRequests: {ex.Message}");
                
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
    }
} 