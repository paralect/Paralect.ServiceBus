using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class QueueMessage
    {
        /// <summary>
        /// Native message format which understood by underlying queue system
        /// </summary>
        public Object Message { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public QueueMessageType MessageType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public QueueMessage(Object message) : this(message, QueueMessageType.Normal) {}
        public QueueMessage(Object message, QueueMessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}
