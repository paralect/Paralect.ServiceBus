using System;

namespace Paralect.ServiceBus.Dispatching2
{
    public interface IHandlerRegistry
    {
        /// <summary>
        /// Returns descriptors in a correct order.
        /// </summary>
        IHandler[] GetHandlers(Type messageType);
    }
}