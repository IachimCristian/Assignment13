namespace InfraSim.Models
{
    public class ServerCapability : IServerCapability
    {
        public virtual long MaximumRequests => 1000;
        
        public virtual int Cost => 2500;
    }
} 