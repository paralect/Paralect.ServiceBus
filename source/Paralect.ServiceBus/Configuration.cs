using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class Configuration
    {
        public EndpointsMapping EndpointsMapping = new EndpointsMapping();
        public QueueName InputQueue { get; set; }
        public QueueName ErrorQueue { get; set; }
        public Int32 NumberOfWorkerThreads { get; set; }
        public Int32 MaxRetries { get; set; }

        public Type MessageHandlerMarkerInterface = typeof(IMessageHandler<>);
    }
}
