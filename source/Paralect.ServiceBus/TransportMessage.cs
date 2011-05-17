using System;

namespace Paralect.ServiceBus
{
    public class TransportMessage
    {
        /// <summary>
        /// Original message received from queue
        /// </summary>
        public Object TransportData { get; set; }

        public String SentFromComputerName { get; set; }
        public String SentFromQueueName { get; set; }
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