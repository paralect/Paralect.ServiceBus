using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus
{
    public interface IMessageHandlerInterceptor
    {
        void Intercept(DispatcherInvocationContext context);
    }
}
