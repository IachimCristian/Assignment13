namespace InfraSim.Models.Server
{
    public class StatusCalculator : IServerVisitor
    {
        public bool IsOK { get; private set; } = true;

        public void Visit(IServer server)
        {
            if (server == null)
                return;
                
            if (server.Validator == null)
                return;
                
            IsOK = IsOK && server.Validator.Validate(server);
        }
    }
} 