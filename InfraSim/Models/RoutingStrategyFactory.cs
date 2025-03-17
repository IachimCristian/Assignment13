using System;
using System.Collections.Generic;

namespace InfraSim.Models
{
    public class RoutingStrategyFactory
    {
        public enum RoutingStrategyType
        {
            CDN,
            Cache,
            Generic
        }

        public static ITrafficRouting CreateRoutingStrategy(RoutingStrategyType strategyType, List<IServer> servers = null)
        {
            servers = servers ?? new List<IServer>();

            switch (strategyType)
            {
                case RoutingStrategyType.CDN:
                    return new CDNTrafficRouting(servers);
                
                case RoutingStrategyType.Cache:
                    return new CacheTrafficRouting(servers);
                
                case RoutingStrategyType.Generic:
                    return new FullTrafficRouting(ServerType.WebServer, servers);
                
                default:
                    throw new ArgumentException($"Unsupported routing strategy type: {strategyType}");
            }
        }
        
        public static ITrafficRouting CreateFullTrafficRouting(ServerType targetServerType, List<IServer> servers = null)
        {
            servers = servers ?? new List<IServer>();
            return new FullTrafficRouting(targetServerType, servers);
        }
    }
} 