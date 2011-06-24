using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Test.Interceptors;
using Paralect.ServiceBus.Test.Messages;
using UnityServiceLocator = Paralect.ServiceLocator.Unity.UnityServiceLocator;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class InterceptorsTest
    {
        [Test]
        public void SimpleTest()
        {
            var inputQueueName1 = new EndpointAddress(Guid.NewGuid().ToString());
            var inputQueueName2 = new EndpointAddress(Guid.NewGuid().ToString());

            ServiceBusConfiguration config1 = null;
            ServiceBusConfiguration config2 = null;

            try
            {
                var unity = new UnityContainer();
                var tracker = new Tracker();
                unity.RegisterInstance(tracker);

                var bus1 = ServiceBus.Create(c => c
                    .SetServiceLocator(new UnityServiceLocator(unity))
                    .MsmqTransport()
                    .SetInputQueue(inputQueueName1.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName2.GetFriendlyName())
                    .Out(out config1)
                );

                var bus2 = ServiceBus.Create(c => c
                    .SetServiceLocator(new UnityServiceLocator(unity))
                    .MsmqTransport()
                    .SetInputQueue(inputQueueName2.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName1.GetFriendlyName())
                    .Dispatcher(d => d
                        .AddHandlers(Assembly.GetExecutingAssembly())
                        .AddInterceptor(typeof(FirstInterceptor))
                        .AddInterceptor(typeof(SecondInterceptor))
                    )
                    .Out(out config2)
                );

                using (bus1)
                using (bus2)
                {
                    bus1.Run();
                    bus2.Run();

                    var msg = new Message1("Hello", 2012);

                    bus1.Send(msg);

                    bus2.Wait();
                    bus1.Wait();

                    Assert.AreEqual(1, tracker.Handlers.Count);
                    Assert.AreEqual(2, tracker.Interceptors.Count);
                    Assert.AreEqual(typeof(Message1), tracker.Handlers[0]);
                    Assert.AreEqual(typeof(FirstInterceptor), tracker.Interceptors[0]);
                    Assert.AreEqual(typeof(SecondInterceptor), tracker.Interceptors[1]);
                }
            }
            catch (Exception ex)
            {
                var z = 45;
                throw;
            }

            finally
            {
                var queueProvider1 = EndpointProviderRegistry.GetQueueProvider(inputQueueName1);
                queueProvider1.DeleteQueue(config1.InputQueue);
                queueProvider1.DeleteQueue(config1.ErrorQueue);

                var queueProvider2 = EndpointProviderRegistry.GetQueueProvider(inputQueueName2);
                queueProvider2.DeleteQueue(config2.InputQueue);
                queueProvider2.DeleteQueue(config2.ErrorQueue);
            }
        }
    }
}
