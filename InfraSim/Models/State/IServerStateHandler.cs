namespace InfraSim.Models.State
{
    public interface IServerStateHandler
    {
        IServerState State { get; set; }
    }
} 