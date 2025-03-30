namespace InfraSim.Models.Server
{
    public interface IServerFactory
    {
        IServer CreateServer();
        IServer CreateCache();
        IServer CreateLoadBalancer();
        IServer CreateCDN();
        IServer CreateCluster();
    }
} 