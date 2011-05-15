using System;

namespace Paralect.ServiceBus
{
    public interface ITransportQueue
    {
        QueueName Name { get; }
        ITransportManager Manager { get; }

        void Purge();
        void Send(TransportMessage message);
        TransportMessage Receive(TimeSpan timeout);
        
    }
}