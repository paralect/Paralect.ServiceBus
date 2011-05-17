namespace Paralect.ServiceBus
{
    public interface IMessageTranslator
    {
        QueueMessage TranslateToQueueMessage(TransportMessage transportMessage);
        TransportMessage TranslateToTransportMessage(QueueMessage queueMessage);
    }
}