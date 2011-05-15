using System;

namespace Paralect.ServiceBus.Bus2
{
    public interface IBusQueue<T> : IObservable<T>
    {
        void Enqueue(T message);
        void Run();
    }
}