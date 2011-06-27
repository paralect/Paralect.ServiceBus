using System;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousTransportEndpoint : ITransportEndpoint, ITransportEndpointObserver
    {
        private readonly TransportEndpointAddress _name;
        private readonly ITransport _transport;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemorySynchronousTransportEndpoint(TransportEndpointAddress name, ITransport transport)
        {
            _name = name;
            _transport = transport;
        }

        public void Dispose()
        {
            
        }

        public TransportEndpointAddress Name
        {
            get { return _name; }
        }

        public event Action<ITransportEndpointObserver> ObserverStarted;
        public event Action<ITransportEndpointObserver> ObserverStopped;
        public event Action<TransportMessage, ITransportEndpointObserver> MessageReceived;

        ITransport ITransportEndpointObserver.Transport
        {
            get { return _transport; }
        }

        public void Start()
        {
        }

        public void Wait()
        {
        }

        ITransport ITransportEndpoint.Transport
        {
            get { return _transport; }
        }

        public void Purge()
        {
        }

        public void Send(TransportMessage message)
        {
            var received = MessageReceived;

            if (received != null)
                received(message, this);
        }

        public TransportMessage Receive(TimeSpan timeout)
        {
            throw new InvalidOperationException("You cannot call Receive() method on Synchronous Queue");
        }
    }
}