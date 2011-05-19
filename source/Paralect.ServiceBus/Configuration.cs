using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    public class Configuration
    {
        /// <summary>
        /// Name of instance of ServiceBus. Used for logging.
        /// </summary>
        private String _name = "Unnamed";

        private HandlerRegistry _handlerRegistry = new HandlerRegistry();
        private EndpointsMapping _endpointsMapping = new EndpointsMapping();

        private IQueueProvider _queueProvider;
        private QueueName _inputQueue;
        private QueueName _errorQueue;
        private Type _messageHandlerMarkerInterface = typeof(IMessageHandler<>);
        private Int32 _numberOfWorkerThreads = 1;
        private Int32 _maxRetries = 1;
        private IUnityContainer _busContainer;

        /// <summary>
        /// Name of instance of ServiceBus. Used for logging.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        public IQueueProvider QueueProvider
        {
            get { return _queueProvider; }
        }

        public HandlerRegistry HandlerRegistry
        {
            get { return _handlerRegistry; }
        }

        public EndpointsMapping EndpointsMapping
        {
            get { return _endpointsMapping; }
        }

        public QueueName InputQueue
        {
            get { return _inputQueue; }
        }

        public QueueName ErrorQueue
        {
            get { return _errorQueue; }
        }

        public Type MessageHandlerMarkerInterface
        {
            get { return _messageHandlerMarkerInterface; }
        }

        public int NumberOfWorkerThreads
        {
            get { return _numberOfWorkerThreads; }
        }

        public int MaxRetries
        {
            get { return _maxRetries; }
        }

        public IUnityContainer BusContainer
        {
            get { return _busContainer; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Configuration(IUnityContainer busContainer)
        {
            _busContainer = busContainer;
        }

        public Configuration AddEndpoint(String typeWildcard, String queueName)
        {
            _endpointsMapping.Map(typeWildcard, queueName, _queueProvider);
            QueueProviderRegistry.Register(new QueueName(queueName), _queueProvider);
            return this;
        }

        public Configuration SetName(String name)
        {
            _name = name;
            return this;
        }

        public Configuration SetInputQueue(String queueName)
        {
            _inputQueue = new QueueName(queueName);

            // If error queue is not defined, set error queue name based on input queue name:
            if (_errorQueue == null)
            {
                var errorQueueName = String.Format("{0}.Errors@{1}", _inputQueue.Name, _inputQueue.ComputerName);
                _errorQueue = new QueueName(errorQueueName);
            }

            return this;
        }

        public Configuration SetErrorQueue(String queueName)
        {
            _errorQueue = new QueueName(queueName);
            return this;
        }

        public Configuration SetHandlerMarkerInterface(Type markerInterface)
        {
            _messageHandlerMarkerInterface = markerInterface;
            return this;
        }

        public Configuration SetNumberOfWorkerThreads(Int32 number)
        {
            _numberOfWorkerThreads = number;
            return this;
        }

        public Configuration SetMaxRetries(Int32 maxRetries)
        {
            _maxRetries = maxRetries;
            return this;
        }

        public Configuration AddHandlers(Assembly assembly, String[] namespaces)
        {
            _handlerRegistry.Register(assembly, namespaces);
            return this;
        }

        public Configuration AddInterceptor(Type interceptor)
        {
            _handlerRegistry.AddInterceptor(interceptor);
            return this;
        }

        public Configuration AddHandlers(Assembly assembly)
        {
            return AddHandlers(assembly, new string[] { });
        }

        public Configuration MsmqTransport()
        {
            _queueProvider = new Msmq.MsmqQueueProvider();
            return this;
        }

        public Configuration MemoryTransport()
        {
            _queueProvider = new InMemory.InMemoryQueueProvider();
            return this;
        }

        public Configuration MemorySynchronousTransport()
        {
            _queueProvider = new InMemory.InMemorySynchronousQueueProvider();
            return this;
        }
    }
}
