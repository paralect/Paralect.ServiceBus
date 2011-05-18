using System;

namespace Paralect.ServiceBus.Dispatching
{
    public class InvocationContext
    {
        private readonly Dispatching.Dispatcher _dispatcher;
        private readonly object _handler;
        private readonly object _message;

        public object Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InvocationContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InvocationContext(Dispatching.Dispatcher dispatcher, Object handler, Object message)
        {
            _dispatcher = dispatcher;
            _handler = handler;
            _message = message;
        }

        public virtual void Invoke()
        {
            _dispatcher.InvokeDynamic(_handler, _message);
        }
    }

    public class InterceptorInvocationContext : InvocationContext
    {
        private readonly IMessageHandlerInterceptor _interceptor;
        private readonly InvocationContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InterceptorInvocationContext(IMessageHandlerInterceptor interceptor, InvocationContext context)
        {
            _interceptor = interceptor;
            _context = context;
        }

        public override void Invoke()
        {
            _interceptor.Intercept(_context);
        }
    }
}