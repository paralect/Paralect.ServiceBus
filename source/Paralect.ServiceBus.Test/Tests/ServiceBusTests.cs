using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class ServiceBusTests
    {
        [Test]
        public void SimpleTest()
        {
            var unity = new UnityContainer();
            var tracker = new Tracker();
            unity.RegisterInstance(tracker);

            var config = new Configuration(unity)
                .SetInputQueue("PSB.App1.Input")
                .SetErrorQueue("PSB.App1.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App2.Input");

            var bus = new ServiceBus(config);
            bus.Run();


            var config2 = new Configuration(unity)
                .SetInputQueue("PSB.App2.Input")
                .SetErrorQueue("PSB.App2.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App1.Input")
                .AddHandlers(Assembly.GetExecutingAssembly());

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
            Assert.AreEqual(typeof(SimpleMessage3), tracker.Handlers[0]);
        }
    }
}
