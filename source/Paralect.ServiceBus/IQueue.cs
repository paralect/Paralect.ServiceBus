using System;

namespace Paralect.ServiceBus
{
    public interface IQueue : IDisposable
    {
        /// <summary>
        /// Queue name
        /// </summary>
        QueueName Name { get; }

        /// <summary>
        /// Queue manager which create this queue
        /// </summary>
        IQueueManager Manager { get; }

        /// <summary>
        /// Delete all messages from this queue
        /// </summary>
        void Purge();

        /// <summary>
        /// Send message to this queue
        /// </summary>
        void Send(QueueMessage message);

        /// <summary>
        /// Blocking call. 
        /// </summary>
        QueueMessage Receive(TimeSpan timeout);

        /// <summary>
        /// Send messages which indicates queue listener shutdown
        /// </summary>
        void SendStopMessages(Int32 count = 1);
    }
}