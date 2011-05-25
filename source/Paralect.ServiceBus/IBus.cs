using System;

namespace Paralect.ServiceBus
{
    public interface IBus : IDisposable
    {
        void Send(params Object[] messages);
        void SendLocal(params Object[] messages);
        void Publish(Object message);

        void Run();
        void Wait();
    }
}