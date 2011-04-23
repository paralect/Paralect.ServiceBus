using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Test.Interceptors;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class InterceptorsTest
    {
        [Test]
        public void SimpleTest()
        {
            var unity = new UnityContainer();
            var tracker = new Tracker();
            unity.RegisterInstance(tracker);

            var config = new Configuration(unity)
                .SetInputQueue("PSB.App1.Input1")
                .SetErrorQueue("PSB.App1.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App2.Input1");

            var bus = new ServiceBus(config);
            bus.Run();

            var config2 = new Configuration(unity)
                .SetInputQueue("PSB.App2.Input1")
                .SetErrorQueue("PSB.App2.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App1.Input1")
                .AddHandlers(Assembly.GetExecutingAssembly())
                .AddInterceptor(typeof(SimpleInterceptor));

            var bus2 = new ServiceBus(config2);
            bus2.Run();

            var msg = new SimpleMessage3
            {
                Id = Guid.NewGuid(),
                Text = "From dddbfdfdgfgusd! Muaha!"
            };

            bus.Send(msg);

            Thread.Sleep(4000);

            Assert.AreEqual(1, tracker.Handlers.Count);
            Assert.AreEqual(1, tracker.Interceptors.Count);
            Assert.AreEqual(typeof(SimpleMessage3), tracker.Handlers[0]);
            Assert.AreEqual(typeof(SimpleInterceptor), tracker.Interceptors[0]);
        }
    }
}
