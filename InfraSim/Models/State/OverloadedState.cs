using InfraSim.Models.Server;
using InfraSim.Models.Health;

namespace InfraSim.Models.State
{
    public class OverloadedState : IServerState
    {
        public void Handle(IServer server) // Handle the overloaded state of the server by checking the health of the server 
        {
            ServerHealthCheck healthCheck = new ServerHealthCheck(server); // Check the health of the server 
            
            if (healthCheck.IsIdle) // If the server is idle, set the state to idle 
            {
                server.State = new IdleState(); // Set the state to idle 
            }
            else if (healthCheck.IsNormal)
            {
                server.State = new NormalState();
            }
            else if (healthCheck.IsFailed)
            {
                server.State = new FailedState();
            }
        }
    }
} 