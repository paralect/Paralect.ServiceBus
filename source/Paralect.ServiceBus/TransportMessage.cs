using System;

namespace Paralect.ServiceBus
{
    public class TransportMessage
    {
        public Object[] Messages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransportMessage(params Object[] messages)
        {
            Messages = messages;
        }
    }
}