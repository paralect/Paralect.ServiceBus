using System;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable
    {
        private readonly IQueueProvider _provider;
        private readonly QueueName _inputQueueName;
        private readonly QueueName _errorQueueName;

        private IQueue _errorQueue;
        private IQueueObserver _queueObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBus(IQueueProvider provider, QueueName inputQueueName, QueueName errorQueueName)
        {
            _provider = provider;
            _inputQueueName = inputQueueName;
            _errorQueueName = errorQueueName;
        }

        public void Start()
        {
            PrepareQueues();

            _queueObserver = _provider.CreateObserver(_inputQueueName);
            _queueObserver.MessageReceived += Observer_MessageReceived;
            _queueObserver.Start();
        }

        void Observer_MessageReceived(QueueMessage queueMessage, IQueueObserver queueObserver)
        {
            try
            {
                var transportMessage = queueObserver.Provider.TranslateToTransportMessage(queueMessage);
            }
            catch (TransportMessageDeserializationException deserializationException)
            {
                _errorQueue.Send(queueMessage);
            }
        }

        private void PrepareQueues()
        {
            String inputMutexName = String.Format("Paralect.ServiceBus.{0}.InputQueue", _inputQueueName.GetFriendlyName());
            if (!_provider.ExistsQueue(_inputQueueName))
            {
                MutexFactory.LockByMutex(inputMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_inputQueueName))
                        _provider.CreateQueue(_inputQueueName);
                });
            }

            String errorMutexName = String.Format("Paralect.ServiceBus.{0}.ErrorQueue", _inputQueueName.GetFriendlyName());
            if (!_provider.ExistsQueue(_errorQueueName))
            {
                MutexFactory.LockByMutex(errorMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_inputQueueName))
                        _provider.CreateQueue(_inputQueueName);

                    if (!_provider.ExistsQueue(_errorQueueName))
                        _errorQueue = _provider.CreateQueue(_inputQueueName);
                });
            }
        }

        public void Wait()
        {
            _queueObserver.Wait();
        }

        public void Dispose()
        {
            if (_queueObserver != null)
            {
                _queueObserver.MessageReceived -= Observer_MessageReceived;
                _queueObserver.Dispose();
            }
        }
    }
}