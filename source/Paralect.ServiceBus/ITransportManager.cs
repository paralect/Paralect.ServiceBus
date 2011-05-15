using System;

namespace Paralect.ServiceBus
{
    public interface ITransportManager
    {
        /// <summary>
        /// Check existence of queue
        /// </summary>
        Boolean Exists(QueueName queueName);

        /// <summary>
        /// Create queue
        /// </summary>
        ITransportQueue Create(QueueName queueName);

        /// <summary>
        /// Delete particular queue
        /// </summary>
        void Delete(QueueName queueName);

        /// <summary>
        /// Open queue
        /// </summary>
        ITransportQueue Open(QueueName queueName);        
    }
}