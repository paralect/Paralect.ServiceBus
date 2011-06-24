using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class EndpointMessage
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
        public EndpointMessageType MessageType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EndpointMessage(Object message) : this(message, Guid.NewGuid().ToString(), "UnnamedMessage", EndpointMessageType.Normal) {}
        public EndpointMessage(Object message, String messageId, String messageName, EndpointMessageType messageType)
        {
            Message = message;
            MessageId = messageId;
            MessageName = messageName;
            MessageType = messageType;
        }
    }
}
