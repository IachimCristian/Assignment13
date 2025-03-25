namespace InfraSim.Models
{
    public abstract class BaseServer : IServer
    {
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
            }
        }

        public ServerType ServerType { get; }
        public IServerCapability ServerCapability { get; }

        protected BaseServer(ServerType serverType, IServerCapability serverCapability)
        {
            ServerType = serverType;
            ServerCapability = serverCapability;
        }

        public void HandleRequests(int requestsCount)
        {
            RequestsCount = requestsCount;
        }
    }
} 