using System;

namespace Paralect.ServiceBus
{
    public interface IQueueObserver
    {
        event Action<Object> NewMessageArrived;
    }
}