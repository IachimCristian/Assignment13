using InfraSim.Models.Server;
using System.Linq;
using System;
using System.Collections.Generic;
using InfraSim.Models.Capability;
using InfraSim.Models.Db;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator 
    {
        public ICluster Gateway { get; private set; }
        public ICluster Processors { get; private set; }
        private readonly ICommandManager _commandManager;
        private readonly IServerDataMapper _serverDataMapper;
        private readonly IServerFactory _serverFactory;
        private readonly ICapabilityFactory _capabilityFactory;
        
        public int TotalCost
        {
            get
            {
                IServerIterator serverIterator = CreateServerIterator();
                var costCalculator = new CostCalculator();
                while (serverIterator.HasNext)
                    serverIterator.Next.Accept(costCalculator);
                return costCalculator.TotalCost;
            }
        }

        public InfrastructureMediator(
            ICommandManager commandManager,
            IServerDataMapper serverDataMapper,
            ICapabilityFactory capabilityFactory,
            IServerFactory serverFactory)
        {
            _commandManager = commandManager;
            _serverDataMapper = serverDataMapper;
            _capabilityFactory = capabilityFactory;
            _serverFactory = serverFactory;
            CreateNewClusters();
            RestoreLastCommand();
        }
        
        private void RestoreLastCommand()
        {
            try
            {
                using (var context = new InfraSimContext())
                {
                    // Get all servers from DB without any ordering relying on Position
                    var dbServers = context.Set<DbServer>().ToList();
                    
                    var commands = new List<ICommand>();

                    foreach (var db in dbServers)
                    {
                        IServer server;
                        switch (db.ServerType)
                        {
                            case ServerType.Cluster:
                                // Skip clusters as they're created in CreateNewClusters
                                continue;
                            case ServerType.CDN:
                                server = _serverFactory.CreateCDN();
                                break;
                            case ServerType.LoadBalancer:
                                server = _serverFactory.CreateLoadBalancer();
                                break;
                            case ServerType.Cache:
                                server = _serverFactory.CreateCache();
                                break;
                            case ServerType.Server:
                                server = _serverFactory.CreateServer();
                                break;
                            default:
                                continue;
                        }
                        
                        server.Id = db.Id;

                        // Add to proper parent cluster based on server type
                        ICluster parent;
                        if (db.ServerType == ServerType.CDN || db.ServerType == ServerType.LoadBalancer)
                        {
                            parent = Gateway;
                        }
                        else
                        {
                            parent = Processors;
                        }
                        
                        // Create command
                        var command = new AddServerCommand(parent, server, _serverDataMapper);
                        commands.Add(command);
                    }

                    // Load commands in order
                    _commandManager.LoadCommands(commands);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RestoreLastCommand: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private ICluster CreateFallbackCluster()
        {
            IServerCapability capability = null;
            
            if (_capabilityFactory != null)
            {
                try
                {
                    capability = _capabilityFactory.Create(ServerType.Cluster);
                }
                catch 
                {
                }
            }
            
            capability = capability ?? new EmptyCapability();
            
            var cluster = new Cluster(capability);
            cluster.Id = Guid.NewGuid();
            
            return cluster;
        }

        private void CreateNewClusters()
        {
            try
            {
                Console.WriteLine("=== CreateNewClusters: Creating new Gateway and Processors clusters ===");
                
                if (_serverFactory == null)
                {
                    Console.WriteLine("ERROR: ServerFactory is null, cannot create clusters");
                    Gateway = CreateFallbackCluster();
                    Processors = CreateFallbackCluster();
                    return;
                }
                
                if (_serverDataMapper == null)
                {
                    Console.WriteLine("ERROR: ServerDataMapper is null, cannot save clusters");
                }
                
                try
                {
                    Gateway = _serverFactory.CreateGatewayCluster();
                    Console.WriteLine($"Created Gateway cluster with ID {Gateway.Id}");
                    
                    if (_serverDataMapper != null)
                    {
                        Console.WriteLine("Saving Gateway to database");
                        _serverDataMapper.Insert(Gateway);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR creating Gateway cluster: {ex.Message}");
                    Gateway = CreateFallbackCluster();
                }
                
                CreateNewProcessorsCluster();
                
                Console.WriteLine($"CreateNewClusters complete: Gateway has {Gateway?.Servers?.Count ?? 0} servers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateNewClusters: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (Gateway == null)
                {
                    Gateway = CreateFallbackCluster();
                }
                
                if (Processors == null)
                {
                    Processors = CreateFallbackCluster();
                    Gateway.AddServer(Processors);
                }
            }
        }
        
        private void CreateNewProcessorsCluster()
        {
            try
            {
                Console.WriteLine("=== CreateNewProcessorsCluster: Creating new Processors cluster ===");
                
                if (_serverFactory == null)
                {
                    Console.WriteLine("ERROR: ServerFactory is null, cannot create processors cluster");
                    Processors = CreateFallbackCluster();
                    return;
                }
                
                try
                {
                    Processors = _serverFactory.CreateProcessorsCluster();
                    Console.WriteLine($"Created Processors cluster with ID {Processors.Id}");
                    
                    if (_serverDataMapper != null)
                    {
                        Console.WriteLine("Saving Processors to database");
                        _serverDataMapper.Insert(Processors);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR creating Processors cluster: {ex.Message}");
                    Processors = CreateFallbackCluster();
                }
                
                if (Gateway != null && Processors != null)
                {
                    Console.WriteLine("Adding Processors to Gateway");
                    Gateway.AddServer(Processors);
                    
                    if (_serverDataMapper != null)
                    {
                        Console.WriteLine("Saving parent-child relationship");
                        _serverDataMapper.AddClusterRelationship(Gateway, Processors);
                    }
                }
                
                Console.WriteLine($"CreateNewProcessorsCluster complete: Processors has {Processors?.Servers?.Count ?? 0} servers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in CreateNewProcessorsCluster: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (Processors == null)
                {
                    Processors = CreateFallbackCluster();
                    
                    if (Gateway != null)
                    {
                        Gateway.AddServer(Processors);
                    }
                }
            }
        }

        public void AddServer(IServer server) 
        {
            if (server.Id == Guid.Empty)
            {
                server.Id = Guid.NewGuid();
            }
            
            switch (server.ServerType)
            {
                case ServerType.CDN:
                case ServerType.LoadBalancer:
                    var addServerCommand = new AddServerCommand(Gateway, server, _serverDataMapper);
                    _commandManager.Execute(addServerCommand);
                    break;
                case ServerType.Cache:
                case ServerType.Server:
                    addServerCommand = new AddServerCommand(Processors, server, _serverDataMapper);
                    _commandManager.Execute(addServerCommand);
                    break;
            }
        }

        public IServerIterator Iterator => new ServerIterator(Gateway);

        public IServerIterator CreateServerIterator()
        {
            return new ServerIterator(Gateway);
        }
    }
} 