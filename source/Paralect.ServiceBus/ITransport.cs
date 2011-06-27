using System;

namespace Paralect.ServiceBus
{
    public interface ITransport
    {
        /// <summary>
        /// Check existence of queue
        /// </summary>
        Boolean ExistsEndpoint(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Delete particular queue
        /// </summary>
        void DeleteEndpoint(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Create queue
        /// </summary>
        void CreateEndpoint(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Prepare local queue
        /// </summary>
        void PrepareEndpoint(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Open queue
        /// </summary>
        ITransportEndpoint OpenEndpoint(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Create new observer
        /// </summary>
        ITransportEndpointObserver CreateObserver(TransportEndpointAddress transportEndpointAddress);

        /// <summary>
        /// Translate from transport message to queue message
        /// </summary>
        TransportMessage TranslateToTransportMessage(ServiceBusMessage serviceBusMessage);

        /// <summary>
        /// Translate from queue message to transport message
        /// </summary>
        ServiceBusMessage TranslateToServiceBusMessage(TransportMessage transportMessage);
    }
}