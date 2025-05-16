namespace InfraSim.Routing
{
    public interface ITrafficRouting
    {
        void RouteTraffic(long requestCount);
        long CalculateRequests(long requestCount);
    }
} 