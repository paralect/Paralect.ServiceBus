using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus.Test.Interceptors
{
    public class SecondInterceptor : IMessageHandlerInterceptor
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Intercept(DispatcherInvocationContext context)
        {
            Tracker.Interceptors.Add(GetType());

            context.Invoke();
        }
    }
}