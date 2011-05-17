using System;

namespace Paralect.ServiceBus
{
    public interface IQueueManager
    {
        IMessageTranslator Translator { get; set; }

        /// <summary>
        /// Check existence of queue
        /// </summary>
        Boolean Exists(QueueName queueName);

        /// <summary>
        /// Delete particular queue
        /// </summary>
        void Delete(QueueName queueName);

        /// <summary>
        /// Create queue
        /// </summary>
        IQueue Create(QueueName queueName);

        /// <summary>
        /// Open queue
        /// </summary>
        IQueue Open(QueueName queueName);
    }
}