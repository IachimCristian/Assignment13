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
        
        public static void RunDecoratorPatternDemo()
        {
            Console.WriteLine("=== Server Capability Decorator Demo ===\n");
            
            var baseCapability = new ServerCapability();
            Console.WriteLine($"Base Capability - Maximum Requests: {baseCapability.MaximumRequests}, Cost: ${baseCapability.Cost}");
            
            var highPerformanceCapability = new HighPerformanceCapability(baseCapability);
            Console.WriteLine($"High Performance - Maximum Requests: {highPerformanceCapability.MaximumRequests}, Cost: ${highPerformanceCapability.Cost}");
            
            var redundancyCapability = new RedundancyCapability(baseCapability);
            Console.WriteLine($"Redundancy - Maximum Requests: {redundancyCapability.MaximumRequests}, Cost: ${redundancyCapability.Cost}");
            
            var securityCapability = new SecurityCapability(baseCapability);
            Console.WriteLine($"Security - Maximum Requests: {securityCapability.MaximumRequests}, Cost: ${securityCapability.Cost}");
            
            var highPerfAndRedundancy = new RedundancyCapability(new HighPerformanceCapability(baseCapability));
            Console.WriteLine($"High Performance + Redundancy - Maximum Requests: {highPerfAndRedundancy.MaximumRequests}, Cost: ${highPerfAndRedundancy.Cost}");
            
            var fullFeatured = new SecurityCapability(new RedundancyCapability(new HighPerformanceCapability(baseCapability)));
            Console.WriteLine($"Full Featured (High Perf + Redundancy + Security) - Maximum Requests: {fullFeatured.MaximumRequests}, Cost: ${fullFeatured.Cost}");
            
            Console.WriteLine("\n=== Additional Capabilities ===");
            
            var temporaryStorageCapability = new TemporaryStorageCapability(baseCapability);
            Console.WriteLine($"Temporary Storage - Maximum Requests: {temporaryStorageCapability.MaximumRequests}, Cost: ${temporaryStorageCapability.Cost}");
            
            var trafficDistributionCapability = new TrafficDistributionCapability(baseCapability);
            Console.WriteLine($"Traffic Distribution - Maximum Requests: {trafficDistributionCapability.MaximumRequests}, Cost: ${trafficDistributionCapability.Cost}");
            
            var edgeServerCapability = new EdgeServerCapability(baseCapability);
            Console.WriteLine($"Edge Server - Maximum Requests: {edgeServerCapability.MaximumRequests}, Cost: ${edgeServerCapability.Cost}");
            
            Console.WriteLine("\n=== Combined Additional Capabilities ===");
            
            var edgeWithHighPerformance = new EdgeServerCapability(new HighPerformanceCapability(baseCapability));
            Console.WriteLine($"Edge Server + High Performance - Maximum Requests: {edgeWithHighPerformance.MaximumRequests}, Cost: ${edgeWithHighPerformance.Cost}");
            
            var distributedEdgeServer = new TrafficDistributionCapability(new EdgeServerCapability(baseCapability));
            Console.WriteLine($"Traffic Distribution + Edge Server - Maximum Requests: {distributedEdgeServer.MaximumRequests}, Cost: ${distributedEdgeServer.Cost}");
            
            Console.WriteLine("\nTesting servers with decorated capabilities:");
            
            var basicServer = new Server
            {
                Name = "Basic Server",
                Type = ServerType.WebServer,
                Capability = baseCapability
            };
            
            var highPerfServer = new Server
            {
                Name = "High Performance Server",
                Type = ServerType.WebServer,
                Capability = highPerformanceCapability
            };
            
            var fullFeaturedServer = new Server
            {
                Name = "Enterprise Server",
                Type = ServerType.WebServer,
                Capability = fullFeatured
            };
            
            var edgeServer = new Server
            {
                Name = "Edge Computing Server",
                Type = ServerType.WebServer,
                Capability = edgeServerCapability
            };
            
            var distributionServer = new Server
            {
                Name = "Traffic Distribution Server",
                Type = ServerType.LoadBalancer,
                Capability = trafficDistributionCapability
            };
            
            int requestCount = 1200;
            Console.WriteLine($"\nSending {requestCount} requests to each server:");
            
            basicServer.HandleRequests(requestCount);
            highPerfServer.HandleRequests(requestCount);
            fullFeaturedServer.HandleRequests(requestCount);
            
            Console.WriteLine("\nSending appropriate requests to specialized servers:");
            edgeServer.HandleRequests(1000);
            distributionServer.HandleRequests(9000);
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
    }
} 