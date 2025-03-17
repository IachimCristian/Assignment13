namespace InfraSim.Models
{
    /// <summary>
    /// Enum representing different types of servers in the infrastructure
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// Standard web server
        /// </summary>
        WebServer,
        
        /// <summary>
        /// Database server
        /// </summary>
        DatabaseServer,
        
        /// <summary>
        /// Cache server for improved performance
        /// </summary>
        CacheServer,
        
        /// <summary>
        /// Authentication server for security
        /// </summary>
        AuthenticationServer,
        
        /// <summary>
        /// Load balancer for distributing traffic
        /// </summary>
        LoadBalancer,
        
        /// <summary>
        /// Content Delivery Network server for distributed content
        /// </summary>
        CDN
    }
} 