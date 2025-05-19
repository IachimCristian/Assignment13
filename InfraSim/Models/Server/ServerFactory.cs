using InfraSim.Models.Capability;
using System;
using System.Linq;
using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public class ServerFactory : IServerFactory
    {
        private readonly ICapabilityFactory _capabilityFactory;
        private readonly ServerBuilder _builder;
        private readonly IServerDataMapper _dataMapper;

        public ServerFactory(ICapabilityFactory capabilityFactory, IServerDataMapper dataMapper)
        {
            _capabilityFactory = capabilityFactory;
            _builder = new ServerBuilder();
            _dataMapper = dataMapper;
        }

        private IServer CreateServerWithType(ServerType type)
        {
            var capability = _capabilityFactory.Create(type);
            return _builder
                .WithId(Guid.NewGuid())
                .WithType(type)
                .WithCapability(capability)
                .WithValidator(new ServerValidator())
                .Build();
        }

        public IServer CreateServer()
        {
            return CreateServerWithType(ServerType.Server);
        }

        public IServer CreateCache()
        {
            return CreateServerWithType(ServerType.Cache);
        }

        public IServer CreateLoadBalancer()
        {
            return CreateServerWithType(ServerType.LoadBalancer);
        }

        public IServer CreateCDN() 
        {
            return CreateServerWithType(ServerType.CDN);
        }

        public ICluster CreateCluster()
        {
            var capability = _capabilityFactory.Create(ServerType.Cluster);
            var cluster = new Cluster(capability, new ServerValidator());
            
            if (cluster.Id == Guid.Empty)
            {
                cluster.Id = Guid.NewGuid();
            }
            
            return cluster;
        }

        public ICluster CreateGatewayCluster()
        {
            try
            {
                var capability = _capabilityFactory.Create(ServerType.Cluster);
                var cluster = new Cluster(capability, new GatewayValidator());
                cluster.Id = Guid.NewGuid();
                
                Console.WriteLine($"=== ServerFactory: Creating gateway cluster with ID {cluster.Id} ===");
                
                var serversFromDb = _dataMapper.GetAll();
                
                if (serversFromDb != null && serversFromDb.Any())
                {
                    Console.WriteLine($"ServerFactory: Found {serversFromDb.Count} servers in database");
                    
                    var gatewayServers = serversFromDb.Where(s => 
                        s.ServerType == ServerType.CDN || 
                        s.ServerType == ServerType.LoadBalancer).ToList();
                    
                    Console.WriteLine($"ServerFactory: Adding {gatewayServers.Count} gateway servers to cluster");
                    foreach (var server in gatewayServers)
                    {
                        if (server.ServerType != ServerType.Cluster && !cluster.Servers.Contains(server))
                        {
                            cluster.AddServer(server);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ServerFactory: No existing servers found for gateway cluster");
                }
                
                return cluster;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateGatewayCluster: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return CreateCluster();
            }
        }

        public ICluster CreateProcessorsCluster()
        {
            try
            {
                var capability = _capabilityFactory.Create(ServerType.Cluster);
                var cluster = new Cluster(capability, new ProcessorsValidator());
                cluster.Id = Guid.NewGuid();
                
                Console.WriteLine($"=== ServerFactory: Creating processors cluster with ID {cluster.Id} ===");
                
                var serversFromDb = _dataMapper.GetAll();
                
                if (serversFromDb != null && serversFromDb.Any())
                {
                    Console.WriteLine($"ServerFactory: Found {serversFromDb.Count} servers in database");
                    
                    var processorServers = serversFromDb.Where(s => 
                        s.ServerType == ServerType.Cache || 
                        s.ServerType == ServerType.Server).ToList();
                    
                    Console.WriteLine($"ServerFactory: Adding {processorServers.Count} processor servers to cluster");
                    foreach (var server in processorServers)
                    {
                        if (server.ServerType != ServerType.Cluster && !cluster.Servers.Contains(server))
                        {
                            cluster.AddServer(server);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ServerFactory: No existing servers found for processors cluster");
                }
                
                return cluster;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateProcessorsCluster: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return CreateCluster();
            }
        }

        public IServer CreateDatabase()
        {
            return CreateServerWithType(ServerType.Database);
        }
        
        public ICluster CreateDataCluster()
        {
            try
            {
                var capability = _capabilityFactory.Create(ServerType.Cluster);
                var cluster = new Cluster(capability, new DataValidator());
                cluster.Id = Guid.NewGuid();
                
                Console.WriteLine($"=== ServerFactory: Creating data cluster with ID {cluster.Id} ===");
                
                var serversFromDb = _dataMapper.GetAll();
                
                if (serversFromDb != null && serversFromDb.Any())
                {
                    Console.WriteLine($"ServerFactory: Found {serversFromDb.Count} servers in database");
                    
                    var databaseServers = serversFromDb.Where(s => 
                        s.ServerType == ServerType.Database).ToList();
                    
                    Console.WriteLine($"ServerFactory: Adding {databaseServers.Count} database servers to cluster");
                    foreach (var server in databaseServers)
                    {
                        if (server.ServerType != ServerType.Cluster && !cluster.Servers.Contains(server))
                        {
                            cluster.AddServer(server);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ServerFactory: No existing servers found for data cluster");
                }
                
                return cluster;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateDataCluster: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return CreateCluster();
            }
        }
    }
} 