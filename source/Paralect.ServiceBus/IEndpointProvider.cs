using System;

namespace Paralect.ServiceBus
{
    public interface IEndpointProvider
    {
        /// <summary>
        /// Check existence of queue
        /// </summary>
        Boolean ExistsQueue(EndpointAddress endpointAddress);

        /// <summary>
        /// Delete particular queue
        /// </summary>
        void DeleteQueue(EndpointAddress endpointAddress);

        /// <summary>
        /// Create queue
        /// </summary>
        void CreateQueue(EndpointAddress endpointAddress);

        /// <summary>
        /// Prepare local queue
        /// </summary>
        void PrepareQueue(EndpointAddress endpointAddress);

        /// <summary>
        /// Open queue
        /// </summary>
        IEndpoint OpenQueue(EndpointAddress endpointAddress);

        /// <summary>
        /// Create new observer
        /// </summary>
        IEndpointObserver CreateObserver(EndpointAddress endpointAddress);

        /// <summary>
        /// Translate from transport message to queue message
        /// </summary>
        EndpointMessage TranslateToQueueMessage(TransportMessage transportMessage);

        /// <summary>
        /// Translate from queue message to transport message
        /// </summary>
        TransportMessage TranslateToTransportMessage(EndpointMessage endpointMessage);
    }
}