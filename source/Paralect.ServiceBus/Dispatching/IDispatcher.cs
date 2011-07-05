using System;

namespace Paralect.ServiceBus.Dispatching
{
    public interface IDispatcher
    {
        void Dispatch(Object message);
    }
}