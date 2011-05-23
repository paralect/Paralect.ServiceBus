using System;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Paralect.ServiceBus
{
    public static class ServiceBusConfigurationExtensions
    {
        public static ServiceBusConfiguration SetName(this ServiceBusConfiguration configuration, string name)
        {
            configuration.Name = name;
            return configuration;
        }

        public static ServiceBusConfiguration AddEndpoint(this ServiceBusConfiguration configuration, String typeWildcard, String queueName)
        {
            configuration.EndpointsMapping.Map(typeWildcard, queueName, configuration.QueueProvider);
//            QueueProviderRegistry.Register(new QueueName(queueName), configuration.QueueProvider);
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

        public static ServiceBusConfiguration SetHandlerMarkerInterface(this ServiceBusConfiguration configuration, Type markerInterface)
        {
            configuration.MessageHandlerMarkerInterface = markerInterface;
            return configuration;
        }

        public static ServiceBusConfiguration SetNumberOfWorkerThreads(this ServiceBusConfiguration configuration, Int32 number)
        {
            configuration.NumberOfWorkerThreads = number;
            return configuration;
        }

        public static ServiceBusConfiguration SetMaxRetries(this ServiceBusConfiguration configuration, Int32 maxRetries)
        {
            configuration.MaxRetries = maxRetries;
            return configuration;
        }

        public static ServiceBusConfiguration AddHandlers(this ServiceBusConfiguration configuration, Assembly assembly, String[] namespaces)
        {
            configuration.HandlerRegistry.Register(assembly, namespaces);
            return configuration;
        }

        public static ServiceBusConfiguration AddInterceptor(this ServiceBusConfiguration configuration, Type interceptor)
        {
            configuration.HandlerRegistry.AddInterceptor(interceptor);
            return configuration;
        }

        public static ServiceBusConfiguration AddHandlers(this ServiceBusConfiguration configuration, Assembly assembly)
        {
            return AddHandlers(configuration, assembly, new string[] { });
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