using System;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IBus, IDisposable
    {
        private readonly Configuration _configuration;
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
        public ServiceBus(Configuration configuration)
        {
            _status = ServiceBusStatus.Stopped;
            _configuration = configuration;
            _provider = configuration.QueueProvider;
            _inputQueueName = configuration.InputQueue;
            _errorQueueName = configuration.ErrorQueue;
        }

        public void Run()
        {
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
                _log.ErrorException("", dispatchingException);
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

            String errorMutexName = String.Format("Paralect.ServiceBus.{0}.ErrorQueue", _inputQueueName.GetFriendlyName());
            if (!_provider.ExistsQueue(_errorQueueName))
            {
                MutexFactory.LockByMutex(errorMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_inputQueueName))
                        _provider.CreateQueue(_inputQueueName);

                    if (!_provider.ExistsQueue(_errorQueueName))
                        _provider.CreateQueue(_inputQueueName);
                });
            }
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

            QueueMessage queueMessage = _provider.TranslateToQueueMessage(transportMessage);

//            var queueNames = _configuration.EndpointsMapping.GetQueues(messages[0].GetType());
            var endpoints = _configuration.EndpointsMapping.GetEndpoints(messages[0].GetType());

            foreach (var endpoint in endpoints)
            {
                var queue = endpoint.QueueProvider.OpenQueue(endpoint.QueueName);
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