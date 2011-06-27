using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousTransport : InMemoryTransport
    {
        /// <summary>
        /// Create new observer
        /// </summary>
        public override ITransportEndpointObserver CreateObserver(TransportEndpointAddress transportEndpointAddress)
        {
            if (!_queues.ContainsKey(transportEndpointAddress.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", transportEndpointAddress.GetFriendlyName()));

            return (ITransportEndpointObserver) _queues[transportEndpointAddress.GetFriendlyName()];
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public override void CreateEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            var queue = new InMemorySynchronousTransportEndpoint(transportEndpointAddress, this);
            _queues[transportEndpointAddress.GetFriendlyName()] = queue;            
        }
    }
}