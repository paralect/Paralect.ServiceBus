using System;

namespace Paralect.ServiceBus
{
    public interface IQueueObserver : IDisposable
    {
        /// <summary>
        /// Event handlers can be invoked in different threads.
        /// </summary>
        event Action<QueueMessage, IQueueObserver> MessageReceived;

        /// <summary>
        /// QueueProvider
        /// </summary>
        IQueueProvider Provider { get; }

        /// <summary>
        /// Start observing
        /// </summary>
        void Start();

        /// <summary>
        /// Wait for shutdown message
        /// </summary>
        void Wait();
    }
}