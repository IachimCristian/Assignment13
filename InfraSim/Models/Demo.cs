using System;
using System.Collections.Generic;
using System.Linq;

namespace InfraSim.Models
{
    public class Demo
    {
        public static void RunCDNRoutingDemo()
        {
            Console.WriteLine("=== CDN Traffic Routing Demo ===");
            
            var servers = new List<IServer>
            {
                new Server { Type = ServerType.CDN, Name = "CDN Server 1" },
                new Server { Type = ServerType.CDN, Name = "CDN Server 2" },
                new Server { Type = ServerType.LoadBalancer, Name = "Load Balancer 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 2" },
                new Server { Type = ServerType.DatabaseServer, Name = "Database Server" }
            };
            
            var cdnRouting = new CDNTrafficRouting(servers);
            
            int totalRequests = 1000;
            Console.WriteLine($"Routing {totalRequests} requests...");
            cdnRouting.RouteTraffic(totalRequests);
            
            Console.WriteLine("\nRouting Results:");
            Console.WriteLine($"CDN Traffic Percentage: {cdnRouting.CDNTrafficPercentage * 100}%");
            Console.WriteLine($"Remaining Traffic Percentage: {cdnRouting.RemainingTrafficPercentage * 100}%");
            
            int cdnRequests = (int)(totalRequests * cdnRouting.CDNTrafficPercentage);
            int remainingRequests = totalRequests - cdnRequests;
            int cdnServersCount = servers.Count(s => s.Type == ServerType.CDN);
            int loadBalancersCount = servers.Count(s => s.Type == ServerType.LoadBalancer);
            
            Console.WriteLine($"\nExpected Distribution:");
            Console.WriteLine($"CDN Servers ({cdnServersCount}): {cdnRequests} requests ({cdnRequests / cdnServersCount} per server)");
            Console.WriteLine($"Load Balancers ({loadBalancersCount}): {remainingRequests} requests ({remainingRequests / loadBalancersCount} per server)");
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
        
        public static void RunCacheRoutingDemo()
        {
            Console.WriteLine("=== Cache Traffic Routing Demo ===");
            
            var servers = new List<IServer>
            {
                new Server { Type = ServerType.CacheServer, Name = "Cache Server 1" },
                new Server { Type = ServerType.CacheServer, Name = "Cache Server 2" },
                new Server { Type = ServerType.LoadBalancer, Name = "Load Balancer 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 2" },
                new Server { Type = ServerType.DatabaseServer, Name = "Database Server" }
            };
            
            var cacheRouting = new CacheTrafficRouting(servers);
            
            int totalRequests = 1000;
            Console.WriteLine($"Routing {totalRequests} requests...");
            cacheRouting.RouteTraffic(totalRequests);
            
            Console.WriteLine("\nRouting Results:");
            Console.WriteLine($"Cache Traffic Percentage: {cacheRouting.CacheTrafficPercentage * 100}%");
            Console.WriteLine($"Remaining Traffic Percentage: {cacheRouting.RemainingTrafficPercentage * 100}%");
            
            int cacheRequests = (int)(totalRequests * cacheRouting.CacheTrafficPercentage);
            int remainingRequests = totalRequests - cacheRequests;
            int cacheServersCount = servers.Count(s => s.Type == ServerType.CacheServer);
            int loadBalancersCount = servers.Count(s => s.Type == ServerType.LoadBalancer);
            
            Console.WriteLine($"\nExpected Distribution:");
            Console.WriteLine($"Cache Servers ({cacheServersCount}): {cacheRequests} requests ({cacheRequests / cacheServersCount} per server)");
            Console.WriteLine($"Load Balancers ({loadBalancersCount}): {remainingRequests} requests ({remainingRequests / loadBalancersCount} per server)");
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
        
        public static void RunFullTrafficRoutingDemo()
        {
            Console.WriteLine("=== Full Traffic Routing Demo ===");
            
            var servers = new List<IServer>
            {
                new Server { Type = ServerType.CDN, Name = "CDN Server 1" },
                new Server { Type = ServerType.CacheServer, Name = "Cache Server 1" },
                new Server { Type = ServerType.LoadBalancer, Name = "Load Balancer 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 2" },
                new Server { Type = ServerType.DatabaseServer, Name = "Database Server" }
            };
            
            var webServerRouting = new FullTrafficRouting(ServerType.WebServer, servers);
            
            int totalRequests = 1000;
            Console.WriteLine($"Routing {totalRequests} requests to Web Servers...");
            webServerRouting.RouteTraffic(totalRequests);
            
            Console.WriteLine("\nCreating a new routing strategy for Database Servers...");
            var dbServerRouting = new FullTrafficRouting(ServerType.DatabaseServer, servers);
            
            Console.WriteLine($"Routing {totalRequests} requests to Database Servers...");
            dbServerRouting.RouteTraffic(totalRequests);
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
        
        public static void RunFactoryDemo()
        {
            Console.WriteLine("=== Routing Strategy Factory Demo ===");
            
            var servers = new List<IServer>
            {
                new Server { Type = ServerType.CDN, Name = "CDN Server 1" },
                new Server { Type = ServerType.CDN, Name = "CDN Server 2" },
                new Server { Type = ServerType.CacheServer, Name = "Cache Server 1" },
                new Server { Type = ServerType.LoadBalancer, Name = "Load Balancer 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 1" },
                new Server { Type = ServerType.WebServer, Name = "Web Server 2" },
                new Server { Type = ServerType.DatabaseServer, Name = "Database Server" }
            };
            
            Console.WriteLine("Creating CDN routing strategy...");
            var cdnStrategy = RoutingStrategyFactory.CreateRoutingStrategy(
                RoutingStrategyFactory.RoutingStrategyType.CDN, 
                servers);
            
            int totalRequests = 1000;
            Console.WriteLine($"Routing {totalRequests} requests using CDN strategy...");
            cdnStrategy.RouteTraffic(totalRequests);
            
            Console.WriteLine("\nCreating Cache routing strategy...");
            var cacheStrategy = RoutingStrategyFactory.CreateRoutingStrategy(
                RoutingStrategyFactory.RoutingStrategyType.Cache, 
                servers);
            
            Console.WriteLine($"Routing {totalRequests} requests using Cache strategy...");
            cacheStrategy.RouteTraffic(totalRequests);
            
            Console.WriteLine("\nCreating Full routing strategy for Web Servers...");
            var webServerStrategy = RoutingStrategyFactory.CreateFullTrafficRouting(
                ServerType.WebServer, 
                servers);
            
            Console.WriteLine($"Routing {totalRequests} requests using Web Server strategy...");
            webServerStrategy.RouteTraffic(totalRequests);
            
            Console.WriteLine("\n=== Factory Demo Complete ===");
        }
    }
    
    public class Server : IServer
    {
        private int _handledRequests;
        
        public ServerType Type { get; set; }
        
        public string Name { get; set; }
        
        public int HandledRequests => _handledRequests;
        
        public void HandleRequests(int requestsCount)
        {
            _handledRequests += requestsCount;
            Console.WriteLine($"{Name} ({Type}) handled {requestsCount} requests. Total: {_handledRequests}");
        }
    }
} 