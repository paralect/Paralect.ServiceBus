using System;
using Microsoft.Practices.ServiceLocation;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Dispatching2
{
    public class Dispatcher : IDispatcher
    {
        /// <summary>
        /// Service Locator that is used to create handlers
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Registry of all registered handlers
        /// </summary>
//        private readonly DispatcherHandlerRegistry _registry;
        private IHandlerRegistry _registry;

        /// <summary>
        /// Number of retries in case exception was logged
        /// </summary>
        private readonly int _maxRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Dispatcher(DispatcherConfiguration configuration)
        {
            if (configuration.ServiceLocator == null)
                throw new ArgumentException("Service Locator is not registered for distributor.");

            _registry = configuration.Builder.BuildHandlerRegistry();
            _serviceLocator = configuration.ServiceLocator;

            /*
            if (configuration.DispatcherHandlerRegistry == null)
                throw new ArgumentException("Dispatcher Handler Registry is null in distributor.");

            
            _registry = configuration.DispatcherHandlerRegistry;
            _maxRetries = configuration.MaxRetries;

            // order handlers 
            _registry.InsureOrderOfHandlers(configuration.Order);*/
        }

        /// <summary>
        /// Factory method
        /// </summary>
        public static Dispatcher Create(Func<DispatcherConfiguration, DispatcherConfiguration> configurationAction)
        {
            var config = new DispatcherConfiguration();
            configurationAction(config);
            return new Dispatcher(config);
        }

        public void Dispatch(Object message)
        {
            Type type = message.GetType();
            IHandler[] handers = _registry.GetHandlers(type);

            foreach (var handler in handers)
            {
                handler.Execute(message, _serviceLocator);
            }
        }

        public void InvokeDynamic(Object handler, Object message)
        {
            dynamic dynamicHandler = handler;
            dynamic dynamicMessage = message;

            dynamicHandler.Handle(dynamicMessage);
        }

        public void InvokeByReflection(Object handler, Object message)
        {
            var methodInfo = handler.GetType().GetMethod("Handle", new[] { message.GetType() });
            methodInfo.Invoke(handler, new [] {message });
        }
    }
}
