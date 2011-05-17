using System;

namespace Paralect.ServiceBus.Exceptions
{
    public class TransportMessageDeserializationException : Exception
    {
        public object ReceivedMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public TransportMessageDeserializationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        /// <param name="receivedMessage">Reference to received original transport message</param>
        public TransportMessageDeserializationException(string message, Object receivedMessage, Exception innerException) : base(message, innerException)
        {
            ReceivedMessage = receivedMessage;
        }
    }
}