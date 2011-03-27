using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.ServiceBus.Dispatcher;

namespace Paralect.ServiceBus
{
    public class Configuration
    {
        private HandlerRegistry _handlerRegistry = new HandlerRegistry();
        private EndpointsMapping _endpointsMapping = new EndpointsMapping();
        private QueueName _inputQueue;
        private QueueName _errorQueue;

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

//        public Int32 NumberOfWorkerThreads { get; set; }
//        public Int32 MaxRetries { get; set; }

        public Type MessageHandlerMarkerInterface = typeof(IMessageHandler<>);

        public Configuration AddEndpoint(String typeWildcard, params String[] queueNames)
        {
            _endpointsMapping.Map(typeWildcard, queueNames);
            return this;
        }

        public Configuration SetInputQueue(String queueName)
        {
            _inputQueue = new QueueName(queueName);
            return this;
        }

        public Configuration SetErrorQueue(String queueName)
        {
            _errorQueue = new QueueName(queueName);
            return this;
        }
    }
}
