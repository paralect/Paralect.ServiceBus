using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class QueueMessage
    {
        /// <summary>
        /// MessageId
        /// </summary>
        public String MessageId { get; set; }

        public String MessageName { get; set; }

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
        public QueueMessage(Object message) : this(message, Guid.NewGuid().ToString(), "UnnamedMessage", QueueMessageType.Normal) {}
        public QueueMessage(Object message, String messageId, String messageName, QueueMessageType messageType)
        {
            Message = message;
            MessageId = messageId;
            MessageName = messageName;
            MessageType = messageType;
        }
    }
}
