using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Paralect.ServiceBus.Dispatching2
{
    public class HandlerRegistryBuilder : IHandlerRegistryBuilder
    {
        /// <summary>
        /// Registered handlers
        /// </summary>
        private readonly OrderedDictionary/* <IHandler, null> */ _handlers = new OrderedDictionary(100);

        /// <summary>
        /// Register handler
        /// </summary>
        public void Register(IHandler handler)
        {
            // Check that handler wasn't registered before
            if (_handlers.Contains(handler))
                throw new Exception(String.Format("Handler {0} already registered.", handler.Name));

            _handlers.Add(handler, null);


        }

        /// <summary>
        /// Unregister handler
        /// </summary>
        public void Unregister(IHandler handler)
        {
            if (!_handlers.Contains(handler))
                throw new Exception(String.Format("Cannot unregister not registered handler. Handler {0} wasn't registered.", handler.Name));

            _handlers.Remove(handler);
        }

        /// <summary>
        /// Build handler registry
        /// </summary>
        public IHandlerRegistry BuildHandlerRegistry()
        {
            Dictionary<Type, List<IHandler>> memory = new Dictionary<Type, List<IHandler>>();

            foreach(DictionaryEntry entry in _handlers)
            {
                IHandler handler = (IHandler) entry.Key;

                foreach (var type in handler.Subscriptions)
                {
                    List<IHandler> memoryHandlers = null;
                    if (!memory.TryGetValue(type, out memoryHandlers))
                    {
                        memoryHandlers = new List<IHandler>();
                        memory[type] = memoryHandlers;
                    }

                    memoryHandlers.Add(handler);
                }
            }

            // Convert to dictionary of arrays
            var result = memory.ToDictionary(p => p.Key, p => p.Value.ToArray());

            return new HandlerRegistry(result);
        }
    }
}