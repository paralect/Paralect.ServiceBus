using System;

namespace Paralect.ServiceBus
{
    public class EndpointDirection
    {
        public Func<Type, Boolean> TypeChecker { get; set; }
        public QueueName QueueName { get; set; }

        /// <summary>
        /// Can be null
        /// </summary>
        public IQueueProvider QueueProvider { get; set; }
    }
}