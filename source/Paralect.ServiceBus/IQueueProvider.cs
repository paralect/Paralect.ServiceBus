using System;

namespace Paralect.ServiceBus
{
    public interface IQueueProvider
    {
        /// <summary>
        /// Check existence of queue
        /// </summary>
        Boolean ExistsQueue(QueueName queueName);

        /// <summary>
        /// Delete particular queue
        /// </summary>
        void DeleteQueue(QueueName queueName);

        /// <summary>
        /// Create queue
        /// </summary>
        IQueue CreateQueue(QueueName queueName);

        /// <summary>
        /// Open queue
        /// </summary>
        IQueue OpenQueue(QueueName queueName);

        /// <summary>
        /// Create new observer
        /// </summary>
        IQueueObserver CreateObserver(QueueName queueName);

        /// <summary>
        /// Translate from transport message to queue message
        /// </summary>
        QueueMessage TranslateToQueueMessage(TransportMessage transportMessage);

        /// <summary>
        /// Translate from queue message to transport message
        /// </summary>
        TransportMessage TranslateToTransportMessage(QueueMessage queueMessage);
    }
}