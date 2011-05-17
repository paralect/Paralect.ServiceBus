using System;

namespace Paralect.ServiceBus
{
    public interface IQueueObserver : IDisposable
    {
        event Action<QueueMessage, IQueueObserver> MessageReceived;
        IQueueManager QueueManager { get; }
        void Start();
        void Wait();
    }
}