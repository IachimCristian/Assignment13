using System;
using System.Collections.Generic;
using InfraSim.Models.Capability;
using InfraSim.Models.State;

namespace InfraSim.Models.Server
{
    /// <summary>
    /// Interface for server components that can handle requests
    /// </summary>
    public interface IServer : IServerStateHandler, IServerAcceptVisit
    {
        Guid Id { get; set; }
        ServerType ServerType { get; }
        IServerCapability ServerCapability { get; }
        int RequestsCount { get; set; }
        IServerState State { get; set; }
        IServerCapability Capability { get; }
        IValidatorStrategy Validator { get; }
        
        /// <summary>
        /// Handles a specified number of requests
        /// </summary>
        /// <param name="requestsCount">The number of requests to handle</param>
        void HandleRequests(int requestsCount);

        void Accept(IServerVisitor visitor);
    }
} 