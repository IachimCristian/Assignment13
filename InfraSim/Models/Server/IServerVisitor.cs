namespace InfraSim.Models.Server
{
    public interface IServerVisitor
    {
        void Visit(IServer server);
    }
} 