using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public interface IQueueTransport
    {
        IQueueObserver QueueObserver { get; }
        /// <summary>
        /// Called before calling any method of transport
        /// </summary>
        void Start();
        void Stop();

        void Send(Object message, QueueName queueName);

//        event Func<Object> MessageArrived; 
    }
}
