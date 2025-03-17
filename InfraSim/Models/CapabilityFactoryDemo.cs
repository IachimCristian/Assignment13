using System;

namespace InfraSim.Models
{
    public class CapabilityFactoryDemo
    {
        public static void Run()
        {
            Console.WriteLine("=== Server Capability Factory Demo ===\n");
            
            var regularServer = new Server(ServerType.WebServer, "Regular Server");
            var cacheServer = new Server(ServerType.CacheServer, "Cache Server");
            var loadBalancer = new Server(ServerType.LoadBalancer, "Load Balancer");
            var cdnServer = new Server(ServerType.CDN, "CDN Server");
            
            Console.WriteLine("Server Capabilities Table:");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("| Server Type    | Maximum Requests | Cost        |");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"| Regular Server | {regularServer.Capability.MaximumRequests,-16} | ${regularServer.Capability.Cost,-10} |");
            Console.WriteLine($"| Cache Server   | {cacheServer.Capability.MaximumRequests,-16} | ${cacheServer.Capability.Cost,-10} |");
            Console.WriteLine($"| Load Balancer  | {loadBalancer.Capability.MaximumRequests,-16} | ${loadBalancer.Capability.Cost,-10} |");
            Console.WriteLine($"| CDN Server     | {cdnServer.Capability.MaximumRequests,-16} | ${cdnServer.Capability.Cost,-10} |");
            Console.WriteLine("--------------------------------------------------");
            
            Console.WriteLine("\nTesting servers with appropriate request loads:");
            
            regularServer.HandleRequests(900);
            
            cacheServer.HandleRequests(90);
            
            loadBalancer.HandleRequests(9000);
            
            cdnServer.HandleRequests(9500);
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
    }
} 