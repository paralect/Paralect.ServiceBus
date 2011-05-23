using System;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IBus, IDisposable
    {
        private readonly ServiceBusConfiguration _configuration;
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly IQueueProvider _provider;
        private readonly QueueName _inputQueueName;
        private readonly QueueName _errorQueueName;

        private IQueue _errorQueue;
        private IQueueObserver _queueObserver;
        private Dispatcher _dispatcher;
        private ServiceBusStatus _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBus(ServiceBusConfiguration configuration)
        {
            if (configuration.BusContainer == null)
                throw new ArgumentException("Unity Container is not registered. User SetUnityContainer() method.");

            if (configuration.InputQueue == null)
                throw new ArgumentException("Input queue not configured. Use SetInputQueue() method.");

            _status = ServiceBusStatus.Stopped;
            _configuration = configuration;
            _provider = configuration.QueueProvider;
            _inputQueueName = configuration.InputQueue;
            _errorQueueName = configuration.ErrorQueue;

            QueueProviderRegistry.Register(_inputQueueName, _provider);
            QueueProviderRegistry.Register(_errorQueueName, _provider);
        }

        public void Run()
        {
            foreach (var endpoint in _configuration.EndpointsMapping.Endpoints)
            {
                if (QueueProviderRegistry.GetQueueProvider(endpoint.QueueName) == null)
                    QueueProviderRegistry.Register(endpoint.QueueName, endpoint.QueueProvider ?? _provider);
            }

            PrepareQueues();
            PrepareDispatcher();

            _errorQueue = _provider.OpenQueue(_errorQueueName);

            _queueObserver = _provider.CreateObserver(_inputQueueName);
            _queueObserver.MessageReceived += Observer_MessageReceived;
            _queueObserver.Start();
            _status = ServiceBusStatus.Running;
        }

        private void PrepareDispatcher()
        {
            _dispatcher = new Dispatcher(
                _configuration.BusContainer,
                _configuration.HandlerRegistry,
                _configuration.MaxRetries);
        }

        void Observer_MessageReceived(QueueMessage queueMessage, IQueueObserver queueObserver)
        {
            try
            {
                var transportMessage = queueObserver.Provider.TranslateToTransportMessage(queueMessage);

                if (transportMessage.Messages == null && transportMessage.Messages.Length < 1)
                    return;

                foreach (var message in transportMessage.Messages)
                {
                    _dispatcher.Dispatch(message);
                }
            }
            catch (DispatchingException dispatchingException)
            {
                _log.ErrorException("Dispatching exception. See logs for more details.", dispatchingException);
                _errorQueue.Send(queueMessage);
            }
            catch (HandlerException handlerException)
            {
                _log.ErrorException("Message handling failed.", handlerException);
                _errorQueue.Send(queueMessage);
            }
            catch (TransportMessageDeserializationException deserializationException)
            {
                _log.ErrorException("Unable to deserialize message #" + queueMessage.MessageId, deserializationException);
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

            _provider.PrepareQueue(_inputQueueName);

            String errorMutexName = String.Format("Paralect.ServiceBus.{0}.ErrorQueue", _inputQueueName.GetFriendlyName());
            if (!_provider.ExistsQueue(_errorQueueName))
            {
                MutexFactory.LockByMutex(errorMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_errorQueueName))
                        _provider.CreateQueue(_errorQueueName);

                });
            }

            _provider.PrepareQueue(_errorQueueName);
        }

        public void Wait()
        {
            _queueObserver.Wait();
            _status = ServiceBusStatus.Stopped;
        }

        public void Dispose()
        {
            if (_queueObserver != null)
            {
                _queueObserver.MessageReceived -= Observer_MessageReceived;

                if (_status != ServiceBusStatus.Stopped)
                    _queueObserver.Dispose();
            }
        }

        public void Send(params object[] messages)
        {
            if (messages == null || messages.Length < 1)
                return;

            TransportMessage transportMessage = new TransportMessage(messages);
            transportMessage.SentFromQueueName = _inputQueueName.GetFriendlyName();

            var endpoints = _configuration.EndpointsMapping.GetEndpoints(messages[0].GetType());

            foreach (var endpoint in endpoints)
            {
                var provider = QueueProviderRegistry.GetQueueProvider(endpoint.QueueName);
                QueueMessage queueMessage = provider.TranslateToQueueMessage(transportMessage);

                var queue = provider.OpenQueue(endpoint.QueueName);

                queue.Send(queueMessage);
            }
        }

        public void SendLocal(params object[] messages)
        {
            TransportMessage transportMessage = new TransportMessage(messages);
            QueueMessage queueMessage = _provider.TranslateToQueueMessage(transportMessage);
            _errorQueue.Send(queueMessage);
        }

        public void Publish(object message)
        {
            Send(message);
        }
    }
}