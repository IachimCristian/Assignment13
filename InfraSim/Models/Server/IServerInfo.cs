namespace InfraSim.Models.Server
{
    public interface IServerInfo
    {
        string Name { get; }
        string ImageUrl { get; }
        string StatusColor { get; }
    }
} 