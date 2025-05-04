using System.Collections.Generic;
using System.Linq;
using System;

namespace InfraSim.Models.Server
{
    public class ListServerProxy : IServerList
    {
        private readonly ICluster _realCluster;
        private readonly IServerDataMapper _dataMapper;

        public ListServerProxy(ICluster realCluster, IServerDataMapper dataMapper)
        {
            _realCluster = realCluster;
            _dataMapper = dataMapper;
        }

        public List<IServer> Servers
        {
            get => _realCluster.Servers;
            set => _realCluster.Servers = value;
        }

        public void AddServer(IServer server)
        {
            try
            {
                // First save the server to the database regardless of whether it's already there
                // This ensures its existence in the database
                _dataMapper.Insert(server);
                    
                // Now add it to the memory collection if it's not already there
                if (!_realCluster.Servers.Any(s => s.Id == server.Id))
                {
                    _realCluster.AddServer(server);
                }
                
                // Always establish the parent-child relationship in the database
                // This is done even if the server was already in the memory collection
                // to ensure the relationship persists
                _dataMapper.AddClusterRelationship(_realCluster, server);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddServer: {ex.Message}");
                
                // Make sure server is in memory even if DB operation failed
                if (!_realCluster.Servers.Any(s => s.Id == server.Id))
                {
                    _realCluster.AddServer(server);
                }
            }
        }

        public void RemoveServer(IServer server)
        {
            try
            {
                // Always try to remove from database
                _dataMapper.Remove(server);
                
                // Remove from memory if present
                var existingServer = _realCluster.Servers.FirstOrDefault(s => s.Id == server.Id);
                if (existingServer != null)
                {
                    _realCluster.RemoveServer(existingServer);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RemoveServer: {ex.Message}");
                
                // Make sure server is removed from memory even if DB operation failed
                var existingServer = _realCluster.Servers.FirstOrDefault(s => s.Id == server.Id);
                if (existingServer != null)
                {
                    _realCluster.RemoveServer(existingServer);
                }
            }
        }
    }
} 