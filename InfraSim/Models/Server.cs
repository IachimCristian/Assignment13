using System;

namespace InfraSim.Models
{
    public class Server : IServer
    {
        private int _handledRequests;
        private static readonly ICapabilityFactory _capabilityFactory = new CapabilityFactory();
        
        public Server()
        {
            Capability = new ServerCapability();
        }
        
        public Server(ServerType type, string name)
        {
            Type = type;
            Name = name;
            Capability = _capabilityFactory.Create(type);
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