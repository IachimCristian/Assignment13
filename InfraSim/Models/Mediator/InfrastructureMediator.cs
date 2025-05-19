using InfraSim.Models.Server;
using System.Linq;
using System;
using System.Collections.Generic;
using InfraSim.Models.Capability;
using InfraSim.Models.Db;
using InfraSim.Routing;

namespace InfraSim.Models.Mediator
{
    public class InfrastructureMediator : IInfrastructureMediator, IObserver
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
        
        public bool IsOK
        {
            get
            {
                try
                {
                    if (Gateway == null)
                        return false;
                        
                    IServerIterator serverIterator = CreateServerIterator(); // Create a new iterator for each check 
                    
                    if (serverIterator == null)
                        return false;
                        
                    var statusCalculator = new StatusCalculator();
                    
                    while (serverIterator.HasNext)
                    {
                        var server = serverIterator.Next;
                        if (server != null)
                        {
                            server.Accept(statusCalculator);
                        }
                    }
                    
                    return statusCalculator.IsOK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking infrastructure status: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    return false; // Return false as a safe default if there's an error
                }
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
                    var dbServers = context.Set<DbServer>().ToList();
                    
                    var commands = new List<ICommand>();

                    foreach (var db in dbServers)
                    {
                        if (db.ServerType == ServerType.Cluster)
                            continue;
                            
                        var command = CreateCommandForServer(db); // Create a new command for each server 
                        if (command != null)
                            commands.Add(command);
                    }

                    _commandManager.LoadCommands(commands);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RestoreLastCommand: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        
        private ICommand CreateCommandForServer(DbServer db) // Create a new command for each server 
        {
            IServer server;
            switch (db.ServerType)
            {
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
                    return null;
            }
            
            server.Id = db.Id;

            ICluster parent; // Set the parent cluster for the server 
            if (db.ServerType == ServerType.CDN || db.ServerType == ServerType.LoadBalancer)
            {
                parent = Gateway;
            }
            else
            {
                parent = Processors;
            }
            
            return new AddServerCommand(parent, server, _serverDataMapper);
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
                
                try
                {
                    Processors = _serverFactory.CreateProcessorsCluster();
                    Console.WriteLine($"Created Processors cluster with ID {Processors.Id}");
                    
                    if (_serverDataMapper != null)
                    {
                        Console.WriteLine("Saving Processors to database");
                        _serverDataMapper.Insert(Processors);
                    }
                    
                    if (Gateway != null && Processors != null)
                    {
                        Console.WriteLine("Adding Processors to Gateway");
                        if (!Gateway.Servers.Contains(Processors))
                        {
                            Gateway.AddServer(Processors);
                        }
                        
                        if (_serverDataMapper != null)
                        {
                            try
                            {
                                Console.WriteLine("Saving parent-child relationship");
                                _serverDataMapper.AddClusterRelationship(Gateway, Processors);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error saving parent-child relationship: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR creating Processors cluster: {ex.Message}");
                    Processors = CreateFallbackCluster();
                    
                    if (Gateway != null && Processors != null && Gateway.Servers != null)
                    {
                        if (!Gateway.Servers.Contains(Processors))
                        {
                            try
                            {
                                Gateway.AddServer(Processors);
                            }
                            catch (Exception innerEx)
                            {
                                Console.WriteLine($"Error adding Processors to Gateway in fallback: {innerEx.Message}");
                            }
                        }
                    }
                }
                
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
                    
                    if (Gateway != null && Processors != null && Gateway.Servers != null)
                    {
                        if (!Gateway.Servers.Contains(Processors))
                        {
                            try 
                            {
                                Gateway.AddServer(Processors);
                            }
                            catch (Exception innerEx)
                            {
                                Console.WriteLine($"Error adding Processors to Gateway in fallback: {innerEx.Message}");
                            }
                        }
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
            if (Gateway == null)
            {
                Console.WriteLine("Warning: Gateway is null when trying to create iterator");
                return new ServerIterator(null);
            }
            
            return new ServerIterator(Gateway);
        }

        public void Update(int users)
        {
            long requestCount = users * 4;
            ITrafficDelivery chain = GetDeliveryChain();
            chain.DeliverRequests(requestCount);
        }

        public ITrafficDelivery GetDeliveryChain()
        {
            ITrafficDelivery CDNDeliveryChain = new CDNTrafficRouting(Gateway.Servers);
            ITrafficDelivery LBDeliveryChain = new FullTrafficRouting(Gateway.Servers, ServerType.LoadBalancer);
            ITrafficDelivery CacheDeliveryChain = new CacheTrafficRouting(Processors.Servers);
            ITrafficDelivery ServerDeliveryChain = new FullTrafficRouting(Processors.Servers, ServerType.Server);
            
            CDNDeliveryChain.SetNext(LBDeliveryChain);
            LBDeliveryChain.SetNext(CacheDeliveryChain);
            CacheDeliveryChain.SetNext(ServerDeliveryChain);
            
            return CDNDeliveryChain;
        }
    }
} 