using System;

namespace Paralect.ServiceBus
{
    public interface IServiceBus : IDisposable
    {
        void Send(params Object[] messages);
        void SendLocal(params Object[] messages);
        void Publish(Object message);

        Exception GetLastException();
        void Run();
        void Wait();
    }
}