using System;
using System.Diagnostics;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IBus
    {
        private readonly ServiceBusConfiguration _configuration;
        

        private readonly IQueueProvider _provider;
        private readonly QueueName _inputQueueName;
        private readonly QueueName _errorQueueName;
        private Exception _lastException;

        private IEndpoint _errorEndpoint;
        private IQueueObserver _queueObserver;
        private Dispatcher _dispatcher;
        private EndpointsMapping _endpointMapping;
        private ServiceBusStatus _status = ServiceBusStatus.Stopped;

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBus(ServiceBusConfiguration configuration)
        {
            if (configuration.BusContainer == null)
                throw new ArgumentException("Unity Container is not registered. User SetUnityContainer() method.");

            if (configuration.InputQueue == null)
                throw new ArgumentException("Input queue not configured. Use SetInputQueue() method.");

            _configuration = configuration;
            _provider = configuration.QueueProvider;
            _inputQueueName = configuration.InputQueue;
            _errorQueueName = configuration.ErrorQueue;
            _endpointMapping = configuration.EndpointsMapping;

            if (_configuration.DispatcherConfiguration.BusContainer == null)
                _configuration.DispatcherConfiguration.BusContainer = configuration.BusContainer;

            QueueProviderRegistry.Register(_inputQueueName, _provider);
            QueueProviderRegistry.Register(_errorQueueName, _provider);
        }

        public void Run()
        {
            foreach (var endpoint in _endpointMapping.Endpoints)
            {
                if (QueueProviderRegistry.GetQueueProvider(endpoint.QueueName) == null)
                    QueueProviderRegistry.Register(endpoint.QueueName, endpoint.QueueProvider ?? _provider);
            }

            PrepareQueues();
            PrepareDispatcher();

            _errorEndpoint = _provider.OpenQueue(_errorQueueName);

            _queueObserver = _provider.CreateObserver(_inputQueueName);
            _queueObserver.MessageReceived += Observer_MessageReceived;
            _queueObserver.Start();
            _status = ServiceBusStatus.Running;
        }

        private void PrepareDispatcher()
        {
            _dispatcher = new Dispatcher(_configuration.DispatcherConfiguration);
        }

        private void Observer_MessageReceived(QueueMessage queueMessage, IQueueObserver queueObserver)
        {
            try
            {
                var transportMessage = queueObserver.Provider.TranslateToTransportMessage(queueMessage);

                if (transportMessage.Messages == null && transportMessage.Messages.Length < 1)
                    return;

                var stop = Stopwatch.StartNew();

                foreach (var message in transportMessage.Messages)
                {
                    _dispatcher.Dispatch(message);
                }
                stop.Stop();
            }
            catch (DispatchingException dispatchingException)
            {
                _lastException = dispatchingException;
                _log.ErrorException("Dispatching exception. See logs for more details.", dispatchingException);
                _errorEndpoint.Send(queueMessage);
            }
            catch (HandlerException handlerException)
            {
                _lastException = handlerException;
                _log.ErrorException("Message handling failed.", handlerException);
                _errorEndpoint.Send(queueMessage);
            }
            catch (TransportMessageDeserializationException deserializationException)
            {
                _lastException = deserializationException;
                _log.ErrorException("Unable to deserialize message #" + queueMessage.MessageId, deserializationException);
                _errorEndpoint.Send(queueMessage);
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

            var endpoints = _endpointMapping.GetEndpoints(messages[0].GetType());

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
            _errorEndpoint.Send(queueMessage);
        }

        public void Publish(object message)
        {
            Send(message);
        }

        /// <summary>
        /// Return last exception or null if no exceptions
        /// </summary>
        public Exception GetLastException()
        {
            return _lastException;
        }

        public static IBus Run(Func<ServiceBusConfiguration, ServiceBusConfiguration> configurationAction)
        {
            var config = new ServiceBusConfiguration();
            configurationAction(config);
            var bus = new ServiceBus(config);
            bus.Run();
            return bus;
        }

        public static IBus Create(Func<ServiceBusConfiguration, ServiceBusConfiguration> configurationAction)
        {
            var config = new ServiceBusConfiguration();
            configurationAction(config);
            return new ServiceBus(config);
        }
    }
}