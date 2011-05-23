namespace Paralect.ServiceBus
{
    public class Endpoint
    {
        public string TypeName { get; set; }
        public QueueName QueueName { get; set; }

        /// <summary>
        /// Can be null
        /// </summary>
        public IQueueProvider QueueProvider { get; set; }
    }
}