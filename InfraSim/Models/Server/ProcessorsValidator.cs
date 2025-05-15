using System.Linq;

namespace InfraSim.Models.Server
{
    public class ProcessorsValidator : IValidatorStrategy
    {
        public bool Validate(IServer server)
        {
            if (server is ICluster cluster)
            {
                bool hasCache = cluster.Servers.Any(s => s.ServerType == ServerType.Cache);
                bool hasServer = cluster.Servers.Any(s => s.ServerType == ServerType.Server);
                return hasCache && hasServer;
            }
            return false;
        }
    }
} 