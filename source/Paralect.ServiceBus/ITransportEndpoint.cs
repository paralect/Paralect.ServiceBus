using System;

namespace Paralect.ServiceBus
{
    public interface ITransportEndpoint : IDisposable
    {
        /// <summary>
        /// Queue name
        /// </summary>
        TransportEndpointAddress Name { get; }

        /// <summary>
        /// Queue manager which create this queue
        /// </summary>
        ITransport Transport { get; }

        /// <summary>
        /// Delete all messages from this queue
        /// </summary>
        void Purge();

        /// <summary>
        /// Send message to this queue
        /// </summary>
        void Send(TransportMessage message);

        /// <summary>
        /// Blocking call. 
        /// </summary>
        TransportMessage Receive(TimeSpan timeout);
    }
}