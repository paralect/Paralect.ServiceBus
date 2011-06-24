using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousEndpointProvider : InMemoryEndpointProvider
    {
        /// <summary>
        /// Create new observer
        /// </summary>
        public override IEndpointObserver CreateObserver(EndpointAddress endpointAddress)
        {
            if (!_queues.ContainsKey(endpointAddress.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", endpointAddress.GetFriendlyName()));

            return (IEndpointObserver) _queues[endpointAddress.GetFriendlyName()];
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public override void CreateQueue(EndpointAddress endpointAddress)
        {
            var queue = new InMemorySynchronousEndpoint(endpointAddress, this);
            _queues[endpointAddress.GetFriendlyName()] = queue;            
        }
    }
}