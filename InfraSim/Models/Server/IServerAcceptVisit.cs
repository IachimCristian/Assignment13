namespace InfraSim.Models.Server
{
    public interface IServerAcceptVisit
    {
        void Accept(IServerVisitor visitor);
    }
} 