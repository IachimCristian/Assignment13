using System.Collections.Generic;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class Cluster : BaseServer, ICluster
    {
        public List<IServer> Servers { get; set; }

        public Cluster(IServerCapability serverCapability) 
            : base(ServerType.Cluster, serverCapability)
        {
            Servers = new List<IServer>();
        } 

        public void AddServer(IServer server)
        {
            Servers.Add(server);
        } 

        public void RemoveServer(IServer server)
        {
            Servers.Remove(server);
        } 

        public override void HandleRequests(int requestsCount) 
        {
            RequestsCount = requestsCount; 
        }
    }
} 