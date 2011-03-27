using System;
using Microsoft.Practices.Unity;

namespace Paralect.ServiceBus.Dispatcher
{
    public class Dispatcher
    {
        private readonly IUnityContainer _container;
        private readonly HandlerRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Dispatcher(IUnityContainer container, HandlerRegistry registry)
        {
            _container = container;
            _registry = registry;
        }

        public void Dispatch(Object message)
        {
            var handlerTypes = _registry.GetHandlersType(message.GetType());

            foreach (var handlerType in handlerTypes)
            {
                var handler = _container.Resolve(handlerType);

//                var methodInfo = handler.GetType().GetMethod("Handle", new[] { message.GetType() });
//                methodInfo.Invoke(handler, new [] {message });

                dynamic dynamicHandler = handler;
                dynamic dynamicMessage = message;

                dynamicHandler.Handle(dynamicMessage);
            }
        }
    }
}
