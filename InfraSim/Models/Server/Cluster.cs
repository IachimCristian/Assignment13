using System.Collections.Generic;
using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class Cluster : BaseServer, ICluster
    {
        public List<IServer> Servers { get; set; }

        public Cluster(IServerCapability capability) 
            : base(ServerType.Cluster, capability)
        {
            Servers = new List<IServer>();
        }

        public Cluster(IServerCapability capability, IValidatorStrategy validator) 
            : base(ServerType.Cluster, capability)
        {
            Servers = new List<IServer>();
            Validator = validator;
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