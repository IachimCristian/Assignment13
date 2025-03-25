namespace InfraSim.Models
{
    public interface IServerStateHandler
    {
        IServerState State { get; set; }
    }
} 