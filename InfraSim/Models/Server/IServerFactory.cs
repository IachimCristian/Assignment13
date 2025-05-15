namespace InfraSim.Models.Server
{
    public interface IServerFactory
    {
        IServer CreateServer();
        IServer CreateCDN();
        IServer CreateLoadBalancer();
        IServer CreateCache();
        ICluster CreateCluster();
        ICluster CreateGatewayCluster();
        ICluster CreateProcessorsCluster();
    }
} 