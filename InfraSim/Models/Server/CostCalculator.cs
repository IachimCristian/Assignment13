namespace InfraSim.Models.Server
{
    public class CostCalculator : IServerVisitor
    {
        public int TotalCost { get; private set; }

        public void Visit(IServer server)
        {
            TotalCost += server.ServerCapability.Cost;
        }
    }
} 