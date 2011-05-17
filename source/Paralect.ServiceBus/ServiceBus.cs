using System;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable
    {
        private QueueObserver _observer;

        public void Start()
        {
            _observer.MessageReceived += Observer_MessageReceived;
        }

        void Observer_MessageReceived(QueueMessage queueMessage, QueueObserver queueObserver)
        {
            
        }

        public void Dispose()
        {
            if (_observer != null)
                _observer.MessageReceived -= Observer_MessageReceived;
        }
    }
}