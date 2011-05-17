using System;
using System.Linq;
using System.Messaging;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqMessageTranslator : IMessageTranslator
    {
        public QueueMessage TranslateToQueueMessage(TransportMessage transportMessage)
        {
            // do not send empty messages
            if (transportMessage.Messages == null || transportMessage.Messages.Length == 0)
                throw new ArgumentException("Transport message is null or empty.");

            Message message = new Message
            {
                Body = transportMessage,
                Formatter = new MsmqMessageFormatter(),
                Label = transportMessage.Messages.First().GetType().FullName
            };

            return new QueueMessage(message);            
        }

        public TransportMessage TranslateToTransportMessage(QueueMessage queueMessage)
        {
            if (queueMessage.Message == null)
                throw new NullReferenceException("QueueMessage.Message is null");

            if (!(queueMessage.Message is System.Messaging.Message))
                throw new ArgumentException("QueueMessage.Message type should be System.Messaging.Message.");

            var message = (System.Messaging.Message)queueMessage.Message;
            var transportMessage = (TransportMessage)ReadMessageBody(message);
            return transportMessage;            
        }

        private Object ReadMessageBody(Message message)
        {
            try { return message.Body; }
            catch (Exception ex)
            {
                throw new TransportMessageDeserializationException("Error when deserializing transport message", message, ex);
            }
        }
    }
}