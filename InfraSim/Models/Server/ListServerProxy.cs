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
                _dataMapper.Insert(server);
                    
                if (!_realCluster.Servers.Any(s => s.Id == server.Id))
                {
                    _realCluster.AddServer(server);
                }
                
                _dataMapper.AddClusterRelationship(_realCluster, server);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddServer: {ex.Message}");
                
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
                _dataMapper.Remove(server);
                
                var existingServer = _realCluster.Servers.FirstOrDefault(s => s.Id == server.Id);
                if (existingServer != null)
                {
                    _realCluster.RemoveServer(existingServer);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RemoveServer: {ex.Message}");
                
                var existingServer = _realCluster.Servers.FirstOrDefault(s => s.Id == server.Id);
                if (existingServer != null)
                {
                    _realCluster.RemoveServer(existingServer);
                }
            }
        }
    }
} 