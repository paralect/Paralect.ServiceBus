using Paralect.ServiceBus.Dispatcher;

namespace Paralect.ServiceBus
{
    public interface IMessageHandlerInterceptor
    {
        void Intercept(InvocationContext context);
    }
}
