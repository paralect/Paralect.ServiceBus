using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.Dispatching2
{
    public class HandlerRegistry : IHandlerRegistry
    {
        /// <summary>
        /// Handlers lookup dictionary. Handlers already in the order.
        /// </summary>
        private readonly Dictionary<Type, IHandler[]> _handlers;

        /// <summary>
        /// Creates HandlerRegistry with specified lookup dictionary
        /// </summary>
        public HandlerRegistry(Dictionary<Type, IHandler[]> handlers)
        {
            _handlers = handlers;
        }

        /// <summary>
        /// Returns handlers in a correct order. 
        /// Returns empty array if no handlers found for such message.
        /// This method doesn't use reflection.
        /// </summary>
        public IHandler[] GetHandlers(Type messageType)
        {
            IHandler[] handlers = null;
            if (!_handlers.TryGetValue(messageType, out handlers))
                return new IHandler[]{};

            return handlers;
        }
    }
}