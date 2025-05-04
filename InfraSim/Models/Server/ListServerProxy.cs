using System.Collections.Generic;

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
            _dataMapper.Insert(server);
            _realCluster.AddServer(server);
        }

        public void RemoveServer(IServer server)
        {
            _dataMapper.Remove(server);
            _realCluster.RemoveServer(server);
        }
    }
} 