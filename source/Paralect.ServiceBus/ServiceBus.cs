using System;
using System.Diagnostics;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Default implementation of IBus 
    /// </summary>
    public class ServiceBus : IBus
    {
        /// <summary>
        /// Configuration settings for ServiceBus
        /// </summary>
        private readonly ServiceBusConfiguration _configuration;
        
        /// <summary>
        /// Endpoint Provider used to send and receive messages
        /// </summary>
        private readonly IEndpointProvider _provider;

        /// <summary>
        /// Input query address from which we are receiving messages
        /// </summary>
        private readonly EndpointAddress _inputEndpointAddress;

        /// <summary>
        /// Error queue address (we are sending to error queue messsages that wasn't handled correctly)
        /// </summary>
        private readonly EndpointAddress _errorEndpointAddress;

        /// <summary>
        /// Error endpoint (we are sending to error queue messsages that wasn't handled correctly)
        /// </summary>
        private IEndpoint _errorEndpoint;

        /// <summary>
        /// Last exception that was "produced" by this service bus
        /// </summary>
        private Exception _lastException;

        /// <summary>
        /// Observer of input queue
        /// </summary>
        private IEndpointObserver _endpointObserver;

        /// <summary>
        /// Dispatcher of messages
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Mapping between endpoints and message type
        /// </summary>
        private EndpointsMapping _endpointMapping;

        /// <summary>
        /// Current status of bus
        /// </summary>
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
            if (configuration.ServiceLocator == null)
                throw new ArgumentException("Unity Container is not registered. User SetUnityContainer() method.");

            if (configuration.InputQueue == null)
                throw new ArgumentException("Input queue not configured. Use SetInputQueue() method.");

            _configuration = configuration;
            _provider = configuration.EndpointProvider;
            _inputEndpointAddress = configuration.InputQueue;
            _errorEndpointAddress = configuration.ErrorQueue;
            _endpointMapping = configuration.EndpointsMapping;

            // use container of ServiceBus, if not specified for dispatcher
            if (_configuration.DispatcherConfiguration.ServiceLocator == null)
                _configuration.DispatcherConfiguration.ServiceLocator = configuration.ServiceLocator;

            EndpointProviderRegistry.Register(_inputEndpointAddress, _provider);
            EndpointProviderRegistry.Register(_errorEndpointAddress, _provider);
        }

        /// <summary>
        /// Run service bus and start input endpoint observing
        /// </summary>
        public void Run()
        {
            // Registration of endpoints
            foreach (var endpoint in _endpointMapping.Endpoints)
            {
                if (EndpointProviderRegistry.GetQueueProvider(endpoint.EndpointAddress) == null)
                    EndpointProviderRegistry.Register(endpoint.EndpointAddress, endpoint.EndpointProvider ?? _provider);
            }

            // Check existence of input and error endpoint and create them if needed
            PrepareQueues();

            // Create dispatcher
            _dispatcher = new Dispatcher(_configuration.DispatcherConfiguration);

            // Open error queue
            _errorEndpoint = _provider.OpenQueue(_errorEndpointAddress);

            // Create and configure observer of input queue
            _endpointObserver = _provider.CreateObserver(_inputEndpointAddress);
            _endpointObserver.MessageReceived += Observer_MessageReceived;
            _endpointObserver.Start();

            // Set servise bus state into Running state 
            _status = ServiceBusStatus.Running;
        }

        /// <summary>
        /// Handle messages that were received from input endpoint
        /// Method can be called from different threads.
        /// </summary>
        private void Observer_MessageReceived(EndpointMessage endpointMessage, IEndpointObserver endpointObserver)
        {
            try
            {
                // Translate to Transport message
                var transportMessage = endpointObserver.Provider.TranslateToTransportMessage(endpointMessage);

                // Ignore transport messages without messages
                if (transportMessage.Messages == null && transportMessage.Messages.Length < 1)
                    return;

                // Dispatch each message (synchronously)
                foreach (var message in transportMessage.Messages)
                {
                    _dispatcher.Dispatch(message);
                }
            }
            catch (DispatchingException dispatchingException)
            {
                _lastException = dispatchingException;
                _log.ErrorException("Dispatching exception. See logs for more details.", dispatchingException);
                _errorEndpoint.Send(endpointMessage);
            }
            catch (HandlerException handlerException)
            {
                _lastException = handlerException;
                _log.ErrorException("Message handling failed.", handlerException);
                _errorEndpoint.Send(endpointMessage);
            }
            catch (TransportMessageDeserializationException deserializationException)
            {
                _lastException = deserializationException;
                _log.ErrorException("Unable to deserialize message #" + endpointMessage.MessageId, deserializationException);
                _errorEndpoint.Send(endpointMessage);
            }
        }

        /// <summary>
        /// Check existence of input and error endpoint and create them if needed
        /// </summary>
        private void PrepareQueues()
        {
            // Prepare input endpoint
            String inputMutexName = String.Format("Paralect.ServiceBus.{0}.InputQueue", _inputEndpointAddress.GetFriendlyName());
            if (!_provider.ExistsQueue(_inputEndpointAddress))
            {
                MutexFactory.LockByMutex(inputMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_inputEndpointAddress))
                        _provider.CreateQueue(_inputEndpointAddress);
                });
            }

            _provider.PrepareQueue(_inputEndpointAddress);


            // Prepare error endpoint
            String errorMutexName = String.Format("Paralect.ServiceBus.{0}.ErrorQueue", _inputEndpointAddress.GetFriendlyName());
            if (!_provider.ExistsQueue(_errorEndpointAddress))
            {
                MutexFactory.LockByMutex(errorMutexName, () =>
                {
                    if (!_provider.ExistsQueue(_errorEndpointAddress))
                        _provider.CreateQueue(_errorEndpointAddress);

                });
            }

            _provider.PrepareQueue(_errorEndpointAddress);
        }

        /// <summary>
        /// Stop service bus and block thread until bus will be stopped.
        /// Maybe rename it to Stop() ?
        /// </summary>
        public void Wait()
        {
            _endpointObserver.Wait();
            _status = ServiceBusStatus.Stopped;
        }

        /// <summary>
        /// Do cleanup logic and stop service bus
        /// </summary>
        public void Dispose()
        {
            if (_endpointObserver != null)
            {
                _endpointObserver.MessageReceived -= Observer_MessageReceived;

                if (_status != ServiceBusStatus.Stopped)
                    _endpointObserver.Dispose();
            }
        }

        /// <summary>
        /// Send message. Recipient resolved by the object type.
        /// </summary>
        public void Send(params object[] messages)
        {
            // Skip if there is no messages
            if (messages == null || messages.Length < 1)
                return;

            // Create transport message
            TransportMessage transportMessage = new TransportMessage(messages);
            transportMessage.SentFromQueueName = _inputEndpointAddress.GetFriendlyName();

            // Get list of endpoints we need send message to
            var endpoints = _endpointMapping.GetEndpoints(messages[0].GetType());

            foreach (var endpoint in endpoints)
            {
                // Create EndpointMessage from TransportMessage
                var provider = EndpointProviderRegistry.GetQueueProvider(endpoint.EndpointAddress);
                EndpointMessage endpointMessage = provider.TranslateToQueueMessage(transportMessage);

                // Send message
                var queue = provider.OpenQueue(endpoint.EndpointAddress);
                queue.Send(endpointMessage);
            }
        }

        /// <summary>
        /// Send message to input queue of this service bus instance.
        /// Can be used for "requeueing" of message.
        /// </summary>
        /// <param name="messages"></param>
        public void SendLocal(params object[] messages)
        {
            TransportMessage transportMessage = new TransportMessage(messages);
            EndpointMessage endpointMessage = _provider.TranslateToQueueMessage(transportMessage);
            _errorEndpoint.Send(endpointMessage);
        }

        /// <summary>
        /// We are using Send() here. Publishing/subscribing not implemented yet.
        /// </summary>
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

        /// <summary>
        /// Factory method. Create and run service bus
        /// </summary>
        public static IBus Run(Func<ServiceBusConfiguration, ServiceBusConfiguration> configurationAction)
        {
            var config = new ServiceBusConfiguration();
            configurationAction(config);
            var bus = new ServiceBus(config);
            bus.Run();
            return bus;
        }

        /// <summary>
        /// Factory method. Create service bus.
        /// </summary>
        public static IBus Create(Func<ServiceBusConfiguration, ServiceBusConfiguration> configurationAction)
        {
            var config = new ServiceBusConfiguration();
            configurationAction(config);
            return new ServiceBus(config);
        }
    }
}