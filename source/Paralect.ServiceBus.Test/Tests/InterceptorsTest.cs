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
            var inputQueueName1 = new QueueName(Guid.NewGuid().ToString());
            var inputQueueName2 = new QueueName(Guid.NewGuid().ToString());

            try
            {
                var unity = new UnityContainer();
                var tracker = new Tracker();
                unity.RegisterInstance(tracker);

                var config1 = new ServiceBusConfiguration()
                    .SetUnityContainer(unity)
                    .MsmqTransport()
                    .SetInputQueue(inputQueueName1.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName2.GetFriendlyName());

                var config2 = new ServiceBusConfiguration()
                    .SetUnityContainer(unity)
                    .MsmqTransport()
                    .SetInputQueue(inputQueueName2.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName1.GetFriendlyName())
                    .AddHandlers(Assembly.GetExecutingAssembly())
                    .AddInterceptor(typeof(FirstInterceptor))
                    .AddInterceptor(typeof(SecondInterceptor));


                using (var bus1 = new ServiceBus(config1))
                using (var bus2 = new ServiceBus(config2))
                {
                    bus1.Run();
                    bus2.Run();

                    var msg = new Message1("Hello", 2012);

                    bus1.Send(msg);

                    bus1.Wait();
                    bus2.Wait();

                    Assert.AreEqual(1, tracker.Handlers.Count);
                    Assert.AreEqual(2, tracker.Interceptors.Count);
                    Assert.AreEqual(typeof(Message1), tracker.Handlers[0]);
                    Assert.AreEqual(typeof(FirstInterceptor), tracker.Interceptors[0]);
                    Assert.AreEqual(typeof(SecondInterceptor), tracker.Interceptors[1]);
                }
            }
            finally
            {
                var queueProvider1 = QueueProviderRegistry.GetQueueProvider(inputQueueName1);
                queueProvider1.DeleteQueue(inputQueueName1);

                var queueProvider2 = QueueProviderRegistry.GetQueueProvider(inputQueueName2);
                queueProvider2.DeleteQueue(inputQueueName2);
            }
        }
    }
}
