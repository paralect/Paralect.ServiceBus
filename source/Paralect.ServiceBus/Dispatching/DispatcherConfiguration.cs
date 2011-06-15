using System;
using Microsoft.Practices.Unity;

namespace Paralect.ServiceBus.Dispatching
{
    public class DispatcherConfiguration
    {
        public DispatcherHandlerRegistry DispatcherHandlerRegistry { get; set; }
        public int MaxRetries { get; set; }
        public IUnityContainer BusContainer { get; set; }
        public Type MessageHandlerMarkerInterface { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherConfiguration()
        {
            DispatcherHandlerRegistry = new DispatcherHandlerRegistry();
            MaxRetries = 1;
        }
    }
}