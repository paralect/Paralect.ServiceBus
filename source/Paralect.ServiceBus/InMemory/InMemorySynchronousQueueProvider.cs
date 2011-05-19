using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousQueueProvider : InMemoryQueueProvider
    {
        /// <summary>
        /// Create new observer
        /// </summary>
        public override IQueueObserver CreateObserver(QueueName queueName)
        {
            if (!_queues.ContainsKey(queueName.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", queueName.GetFriendlyName()));

            return (IQueueObserver) _queues[queueName.GetFriendlyName()];
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public override void CreateQueue(QueueName queueName)
        {
            var queue = new InMemorySynchronousQueue(queueName, this);
            _queues[queueName.GetFriendlyName()] = queue;            
        }
    }
}