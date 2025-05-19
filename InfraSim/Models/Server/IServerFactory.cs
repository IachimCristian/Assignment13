namespace InfraSim.Models.Server
{
    public interface IServerFactory
    {
        IServer CreateServer();
        IServer CreateCDN();
        IServer CreateLoadBalancer();
        IServer CreateCache();
        IServer CreateDatabase();
        ICluster CreateCluster();
        ICluster CreateGatewayCluster();
        ICluster CreateProcessorsCluster();
        ICluster CreateDataCluster();
    }
} 