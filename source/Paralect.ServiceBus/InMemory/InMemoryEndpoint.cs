using System;
using System.Collections.Generic;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryEndpoint : IEndpoint
    {
        private BlockingQueue<EndpointMessage> _messages = new BlockingQueue<EndpointMessage>();

        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly EndpointAddress _name;
        private readonly IEndpointProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryEndpoint(EndpointAddress name, IEndpointProvider provider)
        {
            _name = name;
            _provider = provider;
        }

        public void Dispose()
        {
            
        }

        public EndpointAddress Name
        {
            get { return _name;  }
        }

        public IEndpointProvider Provider
        {
            get { return _provider;  }
        }

        public void Purge()
        {
            _messages.Clear();
        }

        public void Send(EndpointMessage message)
        {
            _messages.Enqueue(message);
        }

        public EndpointMessage Receive(TimeSpan timeout)
        {
            EndpointMessage message;
            var result = _messages.TryDequeue(out message, (int) timeout.TotalMilliseconds);

            if (!result)
                throw new TransportTimeoutException("Timeout when receiving message");

            return message;
        }
    }
}