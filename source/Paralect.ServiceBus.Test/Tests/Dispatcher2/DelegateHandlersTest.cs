using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching2;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Dispatcher2
{
    [TestFixture]
    public class DelegateHandlersTest
    {
        [Test]
        public void Test()
        {
            var unity = Unity();

            var dispatcher = CreateDispatcher(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>())
            );

            dispatcher.Dispatch(new Message1 { Name = "Hello!" });
        }

        public IUnityContainer Unity()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer().RegisterInstance(tracker);

            return unity;
        }

        public IDispatcher CreateDispatcher(Func<DispatcherConfiguration, DispatcherConfiguration> configurationFunc)
        {
            var tracker = new Tracker();
            var unity = new UnityContainer().RegisterInstance(tracker);

            Func<DispatcherConfiguration, DispatcherConfiguration> c = d =>
            {
                d.SetUnityContainer(unity); 
                return configurationFunc(d); 
            };

            return Dispatcher.Create(c);
        }

        public void Ttttest()
        {
            var list = typeof(List<Int32>).GetInterfaces();
        }

    }
}