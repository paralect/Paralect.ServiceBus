using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Configuration of Service Bus.
    /// </summary>
    public class ServiceBusConfiguration
    {
        /// <summary>
        /// Name of instance of ServiceBus. Used for logging.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unity container
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Input endpoint
        /// </summary>
        public TransportEndpointAddress InputQueue { get; set; }

        /// <summary>
        /// Error endpoint
        /// </summary>
        public TransportEndpointAddress ErrorQueue { get; set; }

        /// <summary>
        /// Endpoint provider, used as a single access point to transport
        /// </summary>
        public ITransport Transport { get; set; }

        /// <summary>
        /// Mapping between type -> endpoint
        /// </summary>
        public EndpointsMapping EndpointsMapping { get; set; }

        /// <summary>
        /// Total number of worker threads (1 by default)
        /// </summary>
        public int NumberOfWorkerThreads { get; set; }

        /// <summary>
        /// Configuration of message dispatcher
        /// </summary>
        public DispatcherConfiguration DispatcherConfiguration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBusConfiguration()
        {
            EndpointsMapping = new EndpointsMapping();
            DispatcherConfiguration = new DispatcherConfiguration();

            // Default name of service bus
            Name = "Unnamed";

            // By default we are using one thread
            NumberOfWorkerThreads = 1;
        }
    }
}
