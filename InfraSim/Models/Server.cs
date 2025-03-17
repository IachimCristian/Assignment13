using System;

namespace InfraSim.Models
{
    public class Server : IServer
    {
        private int _handledRequests;
        
        public Server()
        {
            Capability = new ServerCapability();
        }
        
        public ServerType Type { get; set; }
        
        public string Name { get; set; }
        
        public int HandledRequests => _handledRequests;
        
        public IServerCapability Capability { get; set; }
        
        public void HandleRequests(int requestsCount)
        {
            if (requestsCount > Capability.MaximumRequests)
            {
                Console.WriteLine($"Warning: {Name} ({Type}) received {requestsCount} requests but has capacity for {Capability.MaximumRequests}");
            }
            
            _handledRequests += requestsCount;
            Console.WriteLine($"{Name} ({Type}) handled {requestsCount} requests. Total: {_handledRequests}");
        }
    }
} 