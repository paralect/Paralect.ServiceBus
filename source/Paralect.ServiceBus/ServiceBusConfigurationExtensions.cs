using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    public static class ServiceBusConfigurationExtensions
    {
        public static ServiceBusConfiguration SetName(this ServiceBusConfiguration configuration, string name)
        {
            configuration.Name = name;
            return configuration;
        }

        public static ServiceBusConfiguration AddEndpoint(this ServiceBusConfiguration configuration, String typeWildcard, String queueName, IQueueProvider queueProvider = null)
        {
            configuration.EndpointsMapping.Map(type => type.FullName.StartsWith(typeWildcard), queueName, queueProvider);
            return configuration;
        }

        public static ServiceBusConfiguration AddEndpoint(this ServiceBusConfiguration configuration, Func<Type, Boolean> typeChecker, String queueName, IQueueProvider queueProvider = null)
        {
            configuration.EndpointsMapping.Map(typeChecker, queueName, queueProvider);
            return configuration;
        }

        public static ServiceBusConfiguration SetInputQueue(this ServiceBusConfiguration configuration, String queueName)
        {
            configuration.InputQueue = new QueueName(queueName);

            // If error queue is not defined, set error queue name based on input queue name:
            if (configuration.ErrorQueue == null)
            {
                var errorQueueName = String.Format("{0}.Errors@{1}", configuration.InputQueue.Name, configuration.InputQueue.ComputerName);
                configuration.ErrorQueue = new QueueName(errorQueueName);
            }

            return configuration;
        }

        public static ServiceBusConfiguration SetErrorQueue(this ServiceBusConfiguration configuration, String queueName)
        {
            configuration.ErrorQueue = new QueueName(queueName);
            return configuration;
        }

        public static ServiceBusConfiguration SetNumberOfWorkerThreads(this ServiceBusConfiguration configuration, Int32 number)
        {
            configuration.NumberOfWorkerThreads = number;
            return configuration;
        }

        public static ServiceBusConfiguration Dispatcher(this ServiceBusConfiguration configuration, Func<DispatcherConfiguration, DispatcherConfiguration> configurationAction)
        {
            var dispatcherConfiguration = new DispatcherConfiguration();
            dispatcherConfiguration.SetUnityContainer(configuration.BusContainer);
            configurationAction(dispatcherConfiguration);

            configuration.DispatcherConfiguration = dispatcherConfiguration;
            return configuration;
        }

        public static ServiceBusConfiguration MsmqTransport(this ServiceBusConfiguration configuration)
        {
            configuration.QueueProvider = new Msmq.MsmqQueueProvider();
            return configuration;
        }

        public static ServiceBusConfiguration MemoryTransport(this ServiceBusConfiguration configuration)
        {
            configuration.QueueProvider = new InMemory.InMemoryQueueProvider();
            return configuration;
        }

        public static ServiceBusConfiguration MemorySynchronousTransport(this ServiceBusConfiguration configuration)
        {
            configuration.QueueProvider = new InMemory.InMemorySynchronousQueueProvider();
            return configuration;
        }        
        
        public static ServiceBusConfiguration SetUnityContainer(this ServiceBusConfiguration configuration, IUnityContainer container)
        {
            configuration.BusContainer = container;
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