namespace InfraSim.Models.Server
{
    public class StatusCalculator : IServerVisitor
    {
        public bool IsOK { get; private set; } = true;

        public void Visit(IServer server)
        {
            IsOK = IsOK && server.Validator.Validate(server);
        }
    }
} 