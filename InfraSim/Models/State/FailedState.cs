using InfraSim.Models.Server;
using InfraSim.Models.Health;

namespace InfraSim.Models.State
{
    public class FailedState : IServerState
    {
        public void Handle(IServer server)
        {
            ServerHealthCheck healthCheck = new ServerHealthCheck(server);
            
            if (healthCheck.IsIdle)
            {
                server.State = new IdleState();
            }
            else if (healthCheck.IsNormal)
            {
                server.State = new NormalState();
            }
            else if (healthCheck.IsOverloaded)
            {
                server.State = new OverloadedState();
            }
        }
    }
} 