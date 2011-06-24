using System;

namespace Paralect.ServiceBus
{
    public interface IEndpointObserver : IDisposable
    {
        /// <summary>
        /// Observer started
        /// </summary>
        event Action<IEndpointObserver> ObserverStarted;

        /// <summary>
        /// Observer stopped
        /// </summary>
        event Action<IEndpointObserver> ObserverStopped;

        /// <summary>
        /// Event handlers can be invoked in different threads.
        /// </summary>
        event Action<EndpointMessage, IEndpointObserver> MessageReceived;

        /// <summary>
        /// QueueProvider
        /// </summary>
        IEndpointProvider Provider { get; }

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