using System;

namespace InfraSim.Models
{
    public class ServerCapabilityDemo
    {
        public static void RunBasicServerCapabilityDemo()
        {
            Console.WriteLine("=== Basic Server Capability Demo ===\n");
            
            var server = new Server
            {
                Name = "Basic Web Server",
                Type = ServerType.WebServer
            };
            
            Console.WriteLine($"Server: {server.Name}");
            Console.WriteLine($"Type: {server.Type}");
            Console.WriteLine($"Maximum Requests: {server.Capability.MaximumRequests}");
            Console.WriteLine($"Cost: ${server.Capability.Cost}");
            
            Console.WriteLine("\nHandling requests within capacity:");
            server.HandleRequests(800);
            
            Console.WriteLine("\nHandling requests exceeding capacity:");
            server.HandleRequests(1200);
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
    }
} 