using System;
using System.Collections.Generic;
using InfraSim.Models.State;
using InfraSim.Models.Capability;
using InfraSim.Models.Health;

namespace InfraSim.Models.Server
{
    public abstract class BaseServer : IServer, IServerStateHandler
    {
        public Guid Id { get; set; }
        private int Requests = 0;
        public int RequestsCount 
        {
            get
            {
                return Requests;
            }
            set
            {
                Requests = value;
                UpdateState();
            }
        }

        public ServerType ServerType { get; }
        public IServerCapability ServerCapability { get; }
        public IServerState State { get; set; }
        private IServerHealthCheck _healthCheck;

        protected BaseServer(ServerType serverType, IServerCapability serverCapability)
        {
            ServerType = serverType;
            ServerCapability = serverCapability;
            _healthCheck = new ServerHealthCheck(this);
            State = new IdleState();
        }

        public virtual void HandleRequests(int requestsCount)
        {
            RequestsCount = requestsCount;
        }

        private void UpdateState()
        {
            if (State != null)
            {
                State.Handle(this);
            }
        }
    }
} 