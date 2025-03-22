namespace InfraSim.Models
{
    /// <summary>
    /// Interface for traffic routing components that allocate requests among servers
    /// </summary>
    public interface ITrafficRouting
    {
        /// <summary>
        /// Routes a specified number of incoming requests to available servers
        /// </summary>
        /// <param name="requestCount">The number of requests to route</param>
        void RouteTraffic(int requestCount);
    }
} 