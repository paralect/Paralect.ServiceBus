using System;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousEndpoint : IEndpoint, IEndpointObserver
    {
        private readonly EndpointAddress _name;
        private readonly IEndpointProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemorySynchronousEndpoint(EndpointAddress name, IEndpointProvider provider)
        {
            _name = name;
            _provider = provider;
        }

        public void Dispose()
        {
            
        }

        public EndpointAddress Name
        {
            get { return _name; }
        }

        public event Action<IEndpointObserver> ObserverStarted;
        public event Action<IEndpointObserver> ObserverStopped;
        public event Action<EndpointMessage, IEndpointObserver> MessageReceived;

        IEndpointProvider IEndpointObserver.Provider
        {
            get { return _provider; }
        }

        public void Start()
        {
        }

        public void Wait()
        {
        }

        IEndpointProvider IEndpoint.Provider
        {
            get { return _provider; }
        }

        public void Purge()
        {
        }

        public void Send(EndpointMessage message)
        {
            var received = MessageReceived;

            if (received != null)
                received(message, this);
        }

        public EndpointMessage Receive(TimeSpan timeout)
        {
            throw new InvalidOperationException("You cannot call Receive() method on Synchronous Queue");
        }
    }
}