using System.Linq;

namespace InfraSim.Models.Server
{
    public class GatewayValidator : IValidatorStrategy
    {
        public bool Validate(IServer server)
        {
            if (server is ICluster cluster)
            {
                bool hasCDN = cluster.Servers.Any(s => s.ServerType == ServerType.CDN);
                bool hasLoadBalancer = cluster.Servers.Any(s => s.ServerType == ServerType.LoadBalancer);
                return hasCDN && hasLoadBalancer;
            }
            return false;
        }
    }
} 