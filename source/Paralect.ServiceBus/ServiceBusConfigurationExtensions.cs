using System;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    public static class ServiceBusConfigurationExtensions
    {
        /// <summary>
        /// Name of instance of ServiceBus. Used for logging.
        /// </summary>
        public static ServiceBusConfiguration SetName(this ServiceBusConfiguration configuration, string name)
        {
            configuration.Name = name;
            return configuration;
        }

        /// <summary>
        /// Add endpoint mapped by type full name
        /// </summary>
        public static ServiceBusConfiguration AddEndpoint(this ServiceBusConfiguration configuration, String typeWildcard, String queueName, IEndpointProvider endpointProvider = null)
        {
            configuration.EndpointsMapping.Map(type => type.FullName.StartsWith(typeWildcard), queueName, endpointProvider);
            return configuration;
        }

        /// <summary>
        /// Add endpoint, mapped by function
        /// </summary>
        public static ServiceBusConfiguration AddEndpoint(this ServiceBusConfiguration configuration, Func<Type, Boolean> typeChecker, String queueName, IEndpointProvider endpointProvider = null)
        {
            configuration.EndpointsMapping.Map(typeChecker, queueName, endpointProvider);
            return configuration;
        }

        /// <summary>
        /// Set name of Input Endpoint
        /// </summary>
        public static ServiceBusConfiguration SetInputQueue(this ServiceBusConfiguration configuration, String queueName)
        {
            configuration.InputQueue = new EndpointAddress(queueName);

            // If error queue is not defined, set error queue name based on input queue name:
            if (configuration.ErrorQueue == null)
            {
                var errorQueueName = String.Format("{0}.Errors@{1}", configuration.InputQueue.Name, configuration.InputQueue.ComputerName);
                configuration.ErrorQueue = new EndpointAddress(errorQueueName);
            }

            return configuration;
        }

        /// <summary>
        /// Set name of Error Endpoint
        /// </summary>
        public static ServiceBusConfiguration SetErrorQueue(this ServiceBusConfiguration configuration, String queueName)
        {
            configuration.ErrorQueue = new EndpointAddress(queueName);
            return configuration;
        }

        /// <summary>
        /// Set number of worker threads
        /// </summary>
        public static ServiceBusConfiguration SetNumberOfWorkerThreads(this ServiceBusConfiguration configuration, Int32 number)
        {
            configuration.NumberOfWorkerThreads = number;
            return configuration;
        }

        /// <summary>
        /// Configure dispatcher
        /// </summary>
        public static ServiceBusConfiguration Dispatcher(this ServiceBusConfiguration configuration, Func<DispatcherConfiguration, DispatcherConfiguration> configurationAction)
        {
            var dispatcherConfiguration = new DispatcherConfiguration();
            dispatcherConfiguration.SetServiceLocator(configuration.ServiceLocator);
            configurationAction(dispatcherConfiguration);

            configuration.DispatcherConfiguration = dispatcherConfiguration;
            return configuration;
        }

        /// <summary>
        /// Use MSMQ transport
        /// </summary>
        public static ServiceBusConfiguration MsmqTransport(this ServiceBusConfiguration configuration)
        {
            configuration.EndpointProvider = new Msmq.MsmqEndpointProvider();
            return configuration;
        }

        /// <summary>
        /// Use memory transport (async)
        /// </summary>
        public static ServiceBusConfiguration MemoryTransport(this ServiceBusConfiguration configuration)
        {
            configuration.EndpointProvider = new InMemory.InMemoryEndpointProvider();
            return configuration;
        }

        /// <summary>
        /// Use memory transport (sync)
        /// </summary>
        public static ServiceBusConfiguration MemorySynchronousTransport(this ServiceBusConfiguration configuration)
        {
            configuration.EndpointProvider = new InMemory.InMemorySynchronousEndpointProvider();
            return configuration;
        }        
        
        /// <summary>
        /// Set Unity Container
        /// </summary>
        public static ServiceBusConfiguration SetServiceLocator(this ServiceBusConfiguration configuration, IServiceLocator container)
        {
            configuration.ServiceLocator = container;
            return configuration;
        }
      
        public static ServiceBusConfiguration Modify(this ServiceBusConfiguration configuration, Action<ServiceBusConfiguration> action)
        {
            action(configuration);
            return configuration;
        }

        public static ServiceBusConfiguration Out(this ServiceBusConfiguration configuration, out ServiceBusConfiguration outConfiguration)
        {
            outConfiguration = configuration;
            return configuration;
        }
    }
}