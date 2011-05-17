using System;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryMessageTranslator : IMessageTranslator
    {
        public QueueMessage TranslateToQueueMessage(TransportMessage transportMessage)
        {
            return new QueueMessage(transportMessage);
        }

        public TransportMessage TranslateToTransportMessage(QueueMessage queueMessage)
        {
            if (queueMessage.Message == null)
                throw new NullReferenceException("Message is null");

            if (!(queueMessage.Message is TransportMessage))
                throw new ArgumentException("Message should be of type TransportMessage");

            return (TransportMessage) queueMessage.Message;
        }
    }
}