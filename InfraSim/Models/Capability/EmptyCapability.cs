using InfraSim.Models.Server;

namespace InfraSim.Models.Capability
{
    /// <summary>
    /// Fallback capability class for error recovery when normal capability creation fails
    /// </summary>
    public class EmptyCapability : IServerCapability
    {
        public int Cost => 0;
        
        public long MaximumRequests => 0;
    }
} 