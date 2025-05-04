namespace InfraSim.Models.Server
{
    public interface IServerIterator
    {
        bool HasNext { get; }
        IServer Next { get; }
    }
} 