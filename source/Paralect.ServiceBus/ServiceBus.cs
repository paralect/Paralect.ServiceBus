using System;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable
    {
        private IQueue _inputQueue;
        private IQueue _errorQueue;
        private QueueObserver _observer;

        public void Start()
        {
            _observer.MessageReceived += Observer_MessageReceived;
        }

        void Observer_MessageReceived(QueueMessage queueMessage, IQueueObserver queueObserver)
        {
            var transportMessage = queueObserver.Queue.Manager.Translator.TranslateToTransportMessage(queueMessage);

            _errorQueue.Send(queueMessage);

        }

        public void Dispose()
        {
            if (_observer != null)
                _observer.MessageReceived -= Observer_MessageReceived;
        }
    }
}