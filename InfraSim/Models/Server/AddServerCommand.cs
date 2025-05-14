using System;

namespace InfraSim.Models.Server
{
    public class AddServerCommand : ICommand
    {
        private readonly ListServerProxy _proxy;
        private readonly IServer _server;

        public AddServerCommand(IServerList listServer, IServer server, IServerDataMapper dataMapper)
        {
            if (listServer is IServer parentServer && parentServer.Id == Guid.Empty)
            {
                parentServer.Id = Guid.NewGuid();
            }
            
            if (server != null && server.Id == Guid.Empty)
            {
                server.Id = Guid.NewGuid();
            }
            
            _proxy = new ListServerProxy((ICluster)listServer, dataMapper);
            _server = server;
        }

        public void Do()
        {
            try
            {
                Console.WriteLine($"AddServerCommand: Adding server {_server.Id} of type {_server.ServerType} to cluster");
                
                // Ensure the server has a valid ID
                if (_server.Id == Guid.Empty)
                {
                    _server.Id = Guid.NewGuid();
                    Console.WriteLine($"Generated new ID for server: {_server.Id}");
                }
                
                // Use the proxy to add the server, which will persist to the database
                _proxy.AddServer(_server);
                
                Console.WriteLine($"AddServerCommand: Server {_server.Id} added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddServerCommand ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public void Undo()
        {
            _proxy.RemoveServer(_server);
        }

        public void Redo()
        {
            try
            {
                _proxy.AddServer(_server);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during Redo operation: {ex.Message}");
                
                if (!((ICluster)_proxy).Servers.Contains(_server))
                {
                    ((ICluster)_proxy).AddServer(_server);
                }
            }
        }
    }
} 