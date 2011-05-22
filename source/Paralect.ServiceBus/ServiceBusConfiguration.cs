using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    public class ServiceBusConfiguration
    {
        /// <summary>
        /// Name of instance of ServiceBus. Used for logging.
        /// </summary>
        public string Name { get; set; }
        public IQueueProvider QueueProvider { get; set; }
        public HandlerRegistry HandlerRegistry { get; set; }
        public EndpointsMapping EndpointsMapping { get; set; }
        public QueueName InputQueue { get; set; }
        public QueueName ErrorQueue { get; set; }
        public Type MessageHandlerMarkerInterface { get; set; }
        public int NumberOfWorkerThreads { get; set; }
        public int MaxRetries { get; set; }
        public IUnityContainer BusContainer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBusConfiguration()
        {
            Name = "Unnamed";
            HandlerRegistry = new HandlerRegistry();
            EndpointsMapping = new EndpointsMapping();
            MessageHandlerMarkerInterface = typeof(IMessageHandler<>);
            NumberOfWorkerThreads = 1;
            MaxRetries = 1;
        }
    }
}
