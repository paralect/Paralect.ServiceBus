using System;
using System.Collections.Generic;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryTransportEndpoint : ITransportEndpoint
    {
        private BlockingQueue<TransportMessage> _messages = new BlockingQueue<TransportMessage>();

        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly TransportEndpointAddress _name;
        private readonly ITransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryTransportEndpoint(TransportEndpointAddress name, ITransport transport)
        {
            _name = name;
            _transport = transport;
        }

        public void Dispose()
        {
            
        }

        public TransportEndpointAddress Name
        {
            get { return _name;  }
        }

        public ITransport Transport
        {
            get { return _transport;  }
        }

        public void Purge()
        {
            _messages.Clear();
        }

        public void Send(TransportMessage message)
        {
            _messages.Enqueue(message);
        }

        public TransportMessage Receive(TimeSpan timeout)
        {
            TransportMessage message;
            var result = _messages.TryDequeue(out message, (int) timeout.TotalMilliseconds);

            if (!result)
                throw new TransportTimeoutException("Timeout when receiving message");

            return message;
        }
    }
}