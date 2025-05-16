namespace InfraSim.Routing
{
    public interface ITrafficDelivery
    {
        void SetNext(ITrafficDelivery nextHandler);
        void DeliverRequests(long requestCount);
    }
} 