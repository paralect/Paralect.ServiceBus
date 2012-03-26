using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Paralect.ServiceBus.Dispatching2
{
    public class DispatcherConfiguration
    {
        //public DispatcherHandlerRegistry DispatcherHandlerRegistry { get; set; }
        public int NumberOfRetries { get; set; }
        public IServiceLocator ServiceLocator { get; set; }
        public Type MessageHandlerMarkerInterface { get; set; }
        public List<Type> Order { get; set; }
        public HandlerRegistryBuilder Builder { get; set; }

//        public Dictionary<Type, Object[]> 

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherConfiguration()
        {
            Builder = new HandlerRegistryBuilder();
            NumberOfRetries = 1;
            Order = new List<Type>();
        }
    }
}