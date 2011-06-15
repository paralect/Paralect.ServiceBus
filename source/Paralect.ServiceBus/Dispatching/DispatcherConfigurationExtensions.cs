using System;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Paralect.ServiceBus.Dispatching
{
    public static class DispatcherConfigurationExtensions
    {
        public static DispatcherConfiguration SetUnityContainer(this DispatcherConfiguration configuration, IUnityContainer container)
        {
            configuration.BusContainer = container;
            return configuration;
        }

        public static DispatcherConfiguration SetMaxRetries(this DispatcherConfiguration configuration, Int32 maxRetries)
        {
            configuration.MaxRetries = maxRetries;
            return configuration;
        }

        public static DispatcherConfiguration AddHandlers(this DispatcherConfiguration configuration, Assembly assembly, String[] namespaces)
        {
            configuration.DispatcherHandlerRegistry.Register(assembly, namespaces);
            return configuration;
        }

        public static DispatcherConfiguration AddInterceptor(this DispatcherConfiguration configuration, Type interceptor)
        {
            configuration.DispatcherHandlerRegistry.AddInterceptor(interceptor);
            return configuration;
        }

        public static DispatcherConfiguration AddHandlers(this DispatcherConfiguration configuration, Assembly assembly)
        {
            return AddHandlers(configuration, assembly, new string[] { });
        }

        public static DispatcherConfiguration SetHandlerMarkerInterface(this DispatcherConfiguration configuration, Type markerInterface)
        {
            configuration.MessageHandlerMarkerInterface = markerInterface;
            return configuration;
        }
    }
}