namespace InfraSim.Models
{
    public class IdleState : IServerState
    {
        public void Handle(IServer server)
        {
            ServerHealthCheck healthCheck = new ServerHealthCheck(server);
            
            if (healthCheck.IsNormal)
            {
                ((IServerStateHandler)server).State = new NormalState();
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