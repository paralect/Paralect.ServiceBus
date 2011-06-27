using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class TransportMessage
    {
        /// <summary>
        /// MessageId
        /// </summary>
        public String MessageId { get; set; }

        /// <summary>
        /// Message Name
        /// </summary>
        public String MessageName { get; set; }

        /// <summary>
        /// Native message format which understood by underlying queue system
        /// </summary>
        public Object Message { get; set; }

        /// <summary>
        /// Message type
        /// </summary>
        public TransportMessageType MessageType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransportMessage(Object message) : this(message, Guid.NewGuid().ToString(), "UnnamedMessage", TransportMessageType.Normal) {}
        public TransportMessage(Object message, String messageId, String messageName, TransportMessageType messageType)
        {
            Message = message;
            MessageId = messageId;
            MessageName = messageName;
            MessageType = messageType;
        }
    }
}
