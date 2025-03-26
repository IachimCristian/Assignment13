using InfraSim.Models.Server;
using InfraSim.Models.Health;

namespace InfraSim.Models.State
{
    public class IdleState : IServerState
    {
        public void Handle(IServer server)
        {
            ServerHealthCheck healthCheck = new ServerHealthCheck(server);
            
            if (healthCheck.IsNormal)
            {
                server.State = new NormalState();
            }
            else if (healthCheck.IsOverloaded)
            {
                server.State = new OverloadedState();
            }
            else if (healthCheck.IsFailed)
            {
                server.State = new FailedState();
            }
        }
    }
} 