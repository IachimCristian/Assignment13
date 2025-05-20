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

        protected void PassToNext(long requestCount)
        {
            try
            {
                if (NextHandler != null && requestCount > 0)
                {
                    NextHandler.DeliverRequests(requestCount);
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error passing traffic to next handler: {ex.Message}");
            }
        }
    }
} 