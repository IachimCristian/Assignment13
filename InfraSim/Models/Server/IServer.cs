using System.Collections.Generic;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    /// <summary>
    /// Interface for server components that can handle requests
    /// </summary>
    public interface IServer
    {
        ServerType ServerType { get; }
        IServerCapability ServerCapability { get; }
        int RequestsCount { get; }
        
        /// <summary>
        /// Handles a specified number of requests
        /// </summary>
        /// <param name="requestsCount">The number of requests to handle</param>
        void HandleRequests(int requestsCount);
    }
} 