namespace Paralect.ServiceBus
{
    public class Endpoint
    {
        public string TypeName { get; set; }
        public QueueName QueueName { get; set; }
        public IQueueProvider QueueProvider { get; set; }
    }
}