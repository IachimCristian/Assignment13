namespace InfraSim.Routing
{
    public abstract class TrafficDelivery : ITrafficDelivery
    {
        protected ITrafficDelivery NextHandler;

        public void SetNext(ITrafficDelivery nextHandler)
        {
            NextHandler = nextHandler;
        }

        public abstract void DeliverRequests(long requestCount);
    }
} 