namespace InfraSim.Models.Server
{
    public class RemoveServerCommand : ICommand
    {
        private readonly ListServerProxy _proxy;
        private readonly IServer _server;

        public RemoveServerCommand(IServerList listServer, IServer server, IServerDataMapper dataMapper)
        {
            _proxy = new ListServerProxy((ICluster)listServer, dataMapper);
            _server = server;
        }

        public void Do()
        {
            _proxy.RemoveServer(_server);
        }

        public void Undo()
        {
            _proxy.AddServer(_server);
        }

        public void Redo()
        {
            try
            {
                _proxy.RemoveServer(_server);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during Redo operation: {ex.Message}");
                
                if (((ICluster)_proxy).Servers.Contains(_server))
                {
                    ((ICluster)_proxy).RemoveServer(_server);
                }
            }
        }
    }
} 