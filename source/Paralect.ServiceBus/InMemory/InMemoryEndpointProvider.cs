using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryEndpointProvider : IEndpointProvider
    {
        protected Dictionary<String, IEndpoint> _queues = new Dictionary<string, IEndpoint>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryEndpointProvider()
        {
        }

        public bool ExistsQueue(EndpointAddress endpointAddress)
        {
            return _queues.ContainsKey(endpointAddress.GetFriendlyName());
        }

        public void DeleteQueue(EndpointAddress endpointAddress)
        {
            _queues.Remove(endpointAddress.GetFriendlyName());
        }

        public virtual void CreateQueue(EndpointAddress endpointAddress)
        {
            var queue = new InMemoryEndpoint(endpointAddress, this);
            _queues[endpointAddress.GetFriendlyName()] = queue;
        }

        public void PrepareQueue(EndpointAddress endpointAddress)
        {
            // nothing to do here...
        }

        public IEndpoint OpenQueue(EndpointAddress endpointAddress)
        {
            if (!_queues.ContainsKey(endpointAddress.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", endpointAddress.GetFriendlyName()));

            return _queues[endpointAddress.GetFriendlyName()];
        }

        public virtual IEndpointObserver CreateObserver(EndpointAddress endpointAddress)
        {
            return new SingleThreadEndpointObserver(this, endpointAddress);
        }

        public EndpointMessage TranslateToQueueMessage(TransportMessage transportMessage)
        {
            return new EndpointMessage(transportMessage);
        }

        public TransportMessage TranslateToTransportMessage(EndpointMessage endpointMessage)
        {
            if (endpointMessage.Message == null)
                throw new NullReferenceException("Message is null");

            if (!(endpointMessage.Message is TransportMessage))
                throw new ArgumentException("Message should be of type TransportMessage");

            return (TransportMessage)endpointMessage.Message;
        }
    }
}