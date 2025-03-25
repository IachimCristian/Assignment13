namespace InfraSim.Models
{
    public class EdgeServerDecorator : ServerCapabilityDecorator // Edge Server Decorator is a decorator that adds edge server capabilities to the server 
    {
        public EdgeServerDecorator(IServerCapability capability) : base(capability) { } // Constructor for the EdgeServerDecorator class that takes a capability as a parameter and passes it to the base class constructor 

        public override long MaximumRequests => Capability.MaximumRequests * 1000; // Maximum requests for the edge server is 1000 times the maximum requests of the base server 
        public override int Cost => Capability.Cost + 50000; // Cost for the edge server is the cost of the base server plus 50000 
    }
} 