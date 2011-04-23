using System;
using Microsoft.Practices.Unity;

namespace Paralect.ServiceBus.Dispatcher
{
    public class Dispatcher
    {
        private readonly IUnityContainer _container;
        private readonly HandlerRegistry _registry;
        private readonly int _maxRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Dispatcher(IUnityContainer container, HandlerRegistry registry, Int32 maxRetries)
        {
            _container = container;
            _registry = registry;
            _maxRetries = maxRetries;
        }

        public void Dispatch(Object message)
        {
            var handlerTypes = _registry.GetHandlersType(message.GetType());

            foreach (var handlerType in handlerTypes)
            {
                var handler = _container.Resolve(handlerType);

                var attempt = 0;
                while (attempt < _maxRetries)
                {
                    try
                    {
                        var context = new InvocationContext(this, handler, message);

                        if (_registry.Interceptors.Count > 0)
                        {
                            foreach (var interceptorType in _registry.Interceptors)
                            {
                                var interceptor = (IMessageHandlerInterceptor) _container.Resolve(interceptorType);
                                context = new InterceptorInvocationContext(interceptor, context);
                            }
                        }

                        context.Invoke();

//                        InvokeDynamic(handler, message);

                        // message handled correctly - so that should be 
                        // the final attempt
                        attempt = _maxRetries;
                    }
                    catch (Exception exception)
                    {
                        attempt++;

                        if (attempt == _maxRetries)
                            throw new HandlerException(String.Format(
                                "Exception in the handler {0} for message {1}", handler.GetType().FullName, message.GetType().FullName), exception, message);
                    }
                }
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
