using System.Collections.Generic;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class Cluster : BaseServer, ICluster
    {
        public List<IServer> Servers { get; private set; }

        public Cluster(IServerCapability serverCapability) 
            : base(ServerType.Cluster, serverCapability)
        {
            Servers = new List<IServer>();
        } // Cluster does not handle requests directly 

        public void AddServer(IServer server)
        {
            Servers.Add(server);
        } // Cluster will not handle requests directly 

        public void RemoveServer(IServer server)
        {
            Servers.Remove(server);
        } 

        public override void HandleRequests(int requestsCount) // Cluster should not handle requests directly 
        {
            RequestsCount = requestsCount; // RequestsCount is set to the number of requests received 
        }
    }
} 