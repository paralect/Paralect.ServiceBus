using System;

namespace Paralect.ServiceBus
{
    public interface IQueueObserver : IDisposable
    {
        event Action<QueueMessage, IQueueObserver> MessageReceived;
        IQueue Queue { get; }
        void Start();
        void Wait();
    }
}