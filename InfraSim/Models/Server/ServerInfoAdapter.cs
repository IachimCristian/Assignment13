namespace InfraSim.Models.Server
{
    public class ServerInfoAdapter : IServerInfo
    {
        private readonly IServer _server;

        public ServerInfoAdapter(IServer server)
        {
            _server = server;
        }

        public string Name => _server.ServerType switch
        {
            ServerType.Server => "Server",
            ServerType.Cache => "Cache",
            ServerType.LoadBalancer => "Load Balancer",
            ServerType.CDN => "CDN",
            _ => throw new System.NotImplementedException()
        };

        public string ImageUrl => $"/images/{_server.ServerType.ToString().ToLower()}.png";

        public string StatusColor => _server.State.ToString() switch
        {
            "Idle" => "Gray",
            "Normal" => "Blue",
            "Overloaded" => "Orange",
            "Failed" => "Red",
            _ => "Gray"
        };
    }
} 