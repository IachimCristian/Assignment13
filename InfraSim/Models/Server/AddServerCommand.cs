namespace InfraSim.Models.Server
{
    public class AddServerCommand : ICommand
    {
        private readonly ListServerProxy _proxy;
        private readonly IServer _server;

        public AddServerCommand(IServerList listServer, IServer server, IServerDataMapper dataMapper)
        {
            _proxy = new ListServerProxy((ICluster)listServer, dataMapper);
            _server = server;
        }

        public void Do()
        {
            _proxy.AddServer(_server);
        }

        public void Undo()
        {
            _proxy.RemoveServer(_server);
        }

        public void Redo()
        {
            Do();
        }
    }
} 