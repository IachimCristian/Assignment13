using System.Linq;

namespace InfraSim.Models.Server
{
    public class DataValidator : IValidatorStrategy
    {
        public bool Validate(IServer server)
        {
            if (server is ICluster cluster)
            {
                bool hasDatabase = cluster.Servers.Any(s => s.ServerType == ServerType.Database);
                return hasDatabase;
            }
            return false;
        }
    }
} 