using System;

namespace Paralect.ServiceBus
{
    public interface ITransportEndpointObserver : IDisposable
    {
        /// <summary>
        /// Observer started
        /// </summary>
        event Action<ITransportEndpointObserver> ObserverStarted;

        /// <summary>
        /// Observer stopped
        /// </summary>
        event Action<ITransportEndpointObserver> ObserverStopped;

        /// <summary>
        /// Event handlers can be invoked in different threads.
        /// </summary>
        event Action<TransportMessage, ITransportEndpointObserver> MessageReceived;

        /// <summary>
        /// QueueProvider
        /// </summary>
        ITransport Transport { get; }

        /// <summary>
        /// Start observing
        /// </summary>
        void Start();

        /// <summary>
        /// Wait for shutdown message
        /// </summary>
        void Wait();
    }
}