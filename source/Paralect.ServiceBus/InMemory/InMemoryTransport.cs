using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryTransport : ITransport
    {
        protected Dictionary<String, ITransportEndpoint> _queues = new Dictionary<string, ITransportEndpoint>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryTransport()
        {
        }

        public bool ExistsEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            return _queues.ContainsKey(transportEndpointAddress.GetFriendlyName());
        }

        public void DeleteEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            _queues.Remove(transportEndpointAddress.GetFriendlyName());
        }

        public virtual void CreateEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            var queue = new InMemoryTransportEndpoint(transportEndpointAddress, this);
            _queues[transportEndpointAddress.GetFriendlyName()] = queue;
        }

        public void PrepareEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            // nothing to do here...
        }

        public ITransportEndpoint OpenEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            if (!_queues.ContainsKey(transportEndpointAddress.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", transportEndpointAddress.GetFriendlyName()));

            return _queues[transportEndpointAddress.GetFriendlyName()];
        }

        public virtual ITransportEndpointObserver CreateObserver(TransportEndpointAddress transportEndpointAddress)
        {
            return new SingleThreadTransportEndpointObserver(this, transportEndpointAddress);
        }

        public TransportMessage TranslateToTransportMessage(ServiceBusMessage serviceBusMessage)
        {
            return new TransportMessage(serviceBusMessage);
        }

        public ServiceBusMessage TranslateToServiceBusMessage(TransportMessage transportMessage)
        {
            if (transportMessage.Message == null)
                throw new NullReferenceException("Message is null");

            if (!(transportMessage.Message is ServiceBusMessage))
                throw new ArgumentException("Message should be of type TransportMessage");

            return (ServiceBusMessage)transportMessage.Message;
        }
    }
}