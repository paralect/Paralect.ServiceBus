using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatching;

namespace Paralect.ServiceBus.Test.Interceptors
{
    public class FirstInterceptor : IMessageHandlerInterceptor
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Intercept(InvocationContext context)
        {
            Tracker.Interceptors.Add(GetType());
            
            context.Invoke();
        }
    }

    public class SecondInterceptor : IMessageHandlerInterceptor
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Intercept(InvocationContext context)
        {
            Tracker.Interceptors.Add(GetType());

            context.Invoke();
        }
    }
}
