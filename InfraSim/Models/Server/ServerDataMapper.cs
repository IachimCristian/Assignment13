using System;
using System.Collections.Generic;
using System.Linq;
using InfraSim.Models.Db;
using InfraSim.Models.Capability;
using Microsoft.EntityFrameworkCore;

namespace InfraSim.Models.Server
{
    public class ServerDataMapper : IServerDataMapper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICapabilityFactory _capabilityFactory;

        public ServerDataMapper(IUnitOfWork unitOfWork, ICapabilityFactory capabilityFactory)
        {
            _unitOfWork = unitOfWork;
            _capabilityFactory = capabilityFactory;
        }

        public List<IServer> GetAll()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<DbServer>();
                var dbServers = repository.GetAll().ToList();
                
                Console.WriteLine($"=== DIAGNOSTIC: Found {dbServers.Count} servers in database ===");
                foreach (var srv in dbServers)
                {
                    Console.WriteLine($"DB Server: ID={srv.Id}, Type={srv.ServerType}, ParentId={srv.ParentId}");
                }
                
                var servers = new List<IServer>();
                foreach (var db in dbServers)
                {
                    var server = MapToBaseServer(db);
                    servers.Add(server);
                }
                return servers;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving all servers: {ex.Message}");
                Console.WriteLine($"ERROR retrieving servers: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<IServer>();
            }
        }

        public IServer? Get(Guid id)
        {
            try
            {
                var db = _unitOfWork.GetRepository<DbServer>().Get(id);
                if (db == null) return null;
                return MapToBaseServer(db);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving server {id}: {ex.Message}");
                return null;
            }
        }

        private BaseServer MapToBaseServer(DbServer dbServer)
        {
            var capability = _capabilityFactory.Create(dbServer.ServerType);
            
            if (dbServer.ServerType == ServerType.Cluster)
            {
                var cluster = new Cluster(capability);
                cluster.Id = dbServer.Id;
                return cluster;
            }
            else
            {
                var server = new Server(dbServer.ServerType, capability);
                server.Id = dbServer.Id;
                return server;
            }
        }

        public void Insert(IServer server)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<DbServer>();
                var existing = repo.Get(server.Id);

                if (existing == null)
                {
                    var db = new DbServer { Id = server.Id, ServerType = server.ServerType };
                    repo.Insert(db);
                    _unitOfWork.SaveChanges();
                    
                    var inserted = repo.Get(server.Id);
                    if (inserted != null)
                    {
                        Console.WriteLine($"=== DIAGNOSTIC: Successfully inserted server {server.Id}, type={server.ServerType} ===");
                    }
                    else
                    {
                        Console.WriteLine($"=== DIAGNOSTIC: FAILED to insert server {server.Id} - not found after insert ===");
                    }
                }
                else
                {
                    Console.WriteLine($"=== DIAGNOSTIC: Server {server.Id} already exists in database ===");
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                System.Diagnostics.Debug.WriteLine($"Server {server.Id} already exists (concurrent insert)");
                Console.WriteLine($"Duplicate key exception when inserting server {server.Id}: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting server {server.Id}: {ex.Message}");
                Console.WriteLine($"ERROR inserting server {server.Id}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public void AddClusterRelationship(IServer parent, IServer child)
        {
            try
            {
                Console.WriteLine($"=== AddClusterRelationship: Setting ParentId={parent.Id} for child={child.Id} ===");
                var repo = _unitOfWork.GetRepository<DbServer>();
                
                var dbChild = repo.Get(child.Id);
                if (dbChild != null)
                {
                    Console.WriteLine($"Found child server {child.Id} in database, current ParentId={dbChild.ParentId}");
                    
                    dbChild.ParentId = parent.Id;
                    Console.WriteLine($"Updated ParentId to {parent.Id}");
                    
                    repo.Update(dbChild);
                    _unitOfWork.SaveChanges();
                    
                    var verifiedChild = repo.Get(child.Id);
                    if (verifiedChild != null)
                    {
                        Console.WriteLine($"Verified child after update: ID={verifiedChild.Id}, ParentId={verifiedChild.ParentId}");
                        if (verifiedChild.ParentId == parent.Id)
                        {
                            Console.WriteLine("Relationship was correctly saved to database");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Relationship was NOT correctly saved to database!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Child server {child.Id} not found in database, attempting to create it");
                    
                    var newDbChild = new DbServer 
                    { 
                        Id = child.Id, 
                        ServerType = child.ServerType,
                        ParentId = parent.Id
                    };
                    
                    _unitOfWork.Begin();
                    
                    try
                    {
                        Console.WriteLine($"Creating new child with ID={child.Id}, Type={child.ServerType}, ParentId={parent.Id}");
                        repo.Insert(newDbChild);
                        _unitOfWork.SaveChanges();
                        _unitOfWork.Commit();
                        
                        var verifiedNew = repo.Get(child.Id);
                        if (verifiedNew != null)
                        {
                            Console.WriteLine($"Verified new child: ID={verifiedNew.Id}, ParentId={verifiedNew.ParentId}");
                            if (verifiedNew.ParentId == parent.Id)
                            {
                                Console.WriteLine("Relationship was correctly saved to database");
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Relationship was NOT correctly saved to database!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("ERROR: New child not found after insert!");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        _unitOfWork.Rollback();
                        Console.WriteLine($"Error creating child server: {innerEx.Message}");
                        
                        dbChild = repo.Get(child.Id);
                        if (dbChild != null)
                        {
                            Console.WriteLine($"Found child on second attempt, current ParentId={dbChild.ParentId}");
                            dbChild.ParentId = parent.Id;
                            repo.Update(dbChild);
                            _unitOfWork.SaveChanges();
                            
                            var verifiedRetry = repo.Get(child.Id);
                            if (verifiedRetry != null && verifiedRetry.ParentId == parent.Id)
                            {
                                Console.WriteLine("Relationship was correctly saved to database on retry");
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Relationship was NOT correctly saved on retry!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding cluster relationship: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public List<IServer> GetClusterChildren(Guid parentId)
        {
            try
            {
                var allServers = _unitOfWork.GetRepository<DbServer>()
                    .GetAll()
                    .Where(s => s.ParentId == parentId)
                    .ToList();
                
                var builder = new ServerBuilder();
                var servers = new List<IServer>();
                
                foreach (var db in allServers)
                {
                    servers.Add(builder
                        .WithId(db.Id)
                        .WithType(db.ServerType)
                        .WithCapability(_capabilityFactory.Create(db.ServerType))
                        .Build());
                }
                
                return servers;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting cluster children: {ex.Message}");
                return new List<IServer>();
            }
        }

        public void Remove(IServer server)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<DbServer>();
                var db = repository.Get(server.Id);
                if (db != null)
                {
                    repository.Delete(db);
                    _unitOfWork.SaveChanges();
                    Console.WriteLine($"Successfully removed server {server.Id} from database");
                }
                else
                {
                    Console.WriteLine($"Server {server.Id} not found in database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR removing server {server.Id}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        
        public bool RemoveAll()
        {
            try
            {
                using (var context = new InfraSimContext())
                {
                    Console.WriteLine("=== ServerDataMapper.RemoveAll: Resetting all server data ===");
                    
                    Console.WriteLine("Setting all ParentIds to NULL...");
                    var nullParentIdCommand = "UPDATE DbServers SET ParentId = NULL";
                    context.Database.ExecuteSqlRaw(nullParentIdCommand);
                    
                    Console.WriteLine("Deleting all servers from database...");
                    var deleteCommand = "DELETE FROM DbServers";
                    var rowsAffected = context.Database.ExecuteSqlRaw(deleteCommand);
                    
                    Console.WriteLine($"RemoveAll: Deleted {rowsAffected} server records from database");
                    
                    var remainingServers = context.DbServers.Count();
                    if (remainingServers > 0)
                    {
                        Console.WriteLine($"WARNING: After RemoveAll, {remainingServers} servers still exist in the database");
                        
                        Console.WriteLine("Attempting to remove servers using RemoveRange...");
                        var allServers = context.DbServers.ToList();
                        context.DbServers.RemoveRange(allServers);
                        context.SaveChanges();
                        
                        remainingServers = context.DbServers.Count();
                        Console.WriteLine($"After RemoveRange: {remainingServers} servers remaining in database");
                    }
                    
                    return remainingServers == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in RemoveAll: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
} 