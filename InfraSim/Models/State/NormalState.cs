using InfraSim.Models.Server;
using InfraSim.Models.Health;

namespace InfraSim.Models.State
{
    public class NormalState : IServerState
    {
        public void Handle(IServer server)
        {
            ServerHealthCheck healthCheck = new ServerHealthCheck(server);
            
            if (healthCheck.IsIdle)
            {
                ((IServerStateHandler)server).State = new IdleState();
            }
            else if (healthCheck.IsOverloaded)
            {
                ((IServerStateHandler)server).State = new OverloadedState();
            }
            else if (healthCheck.IsFailed)
            {
                ((IServerStateHandler)server).State = new FailedState();
            }
        }
    }
} 