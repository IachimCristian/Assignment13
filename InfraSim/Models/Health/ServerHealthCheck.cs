using InfraSim.Models.Server;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Health
{
    public class ServerHealthCheck : IServerHealthCheck
    {
        private readonly IServer _server;

        public ServerHealthCheck(IServer server)
        {
            _server = server;
        }

        public bool IsIdle => _server.RequestsCount == 0;

        public bool IsNormal => _server.RequestsCount > 0 && GetLoadPercentage() < 80;

        public bool IsOverloaded => GetLoadPercentage() >= 80 && GetLoadPercentage() < 100;

        public bool IsFailed => GetLoadPercentage() >= 100;

        private double GetLoadPercentage()
        {
            if (_server.ServerCapability.MaximumRequests == 0)
                return 100; 

            return ((double)_server.RequestsCount / _server.ServerCapability.MaximumRequests) * 100;
        }
    }
} 