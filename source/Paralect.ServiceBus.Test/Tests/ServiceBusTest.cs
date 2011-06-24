using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Test.Messages;
using UnityServiceLocator = Paralect.ServiceLocator.Unity.UnityServiceLocator;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class ServiceBusTest
    {
        [Test]
        public void MsmqServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MsmqTransport(),
                config2 => config2.MsmqTransport());
        }

        [Test]
        public void MemoryServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MemoryTransport(),
                config2 => config2.MemoryTransport());
        }

        [Test]
        public void MemorySyncServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MemorySynchronousTransport(),
                config2 => config2.MemorySynchronousTransport());
        }

        [Test]
        public void MsmqAndMemoryServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MsmqTransport(),
                config2 => config2.MemoryTransport());
        }

        [Test]
        public void MsmqAndMemorySyncServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MsmqTransport(),
                config2 => config2.MemorySynchronousTransport());
        }


        [Test]
        public void SynchronousToMemoryTest()
        {
            TestTwoBus(
                config1 => config1.MemoryTransport(),
                config2 => config2.MemorySynchronousTransport());
        }

        [Test]
        public void MemoryAndMsmqServiceBusTest()
        {
            TestTwoBus(
                config1 => config1.MemoryTransport(),
                config2 => config2.MsmqTransport());
        }

        [Test]
        public void MemoryToSynchronousTest()
        {
            TestTwoBus(
                config1 => config1.MemorySynchronousTransport(),
                config2 => config2.MemoryTransport());
        }

        [Test]
        public void MemorySynchronousToMsmqTest()
        {
            TestTwoBus(
                config1 => config1.MemorySynchronousTransport(),
                config2 => config2.MsmqTransport());
        }


        private void TestTwoBus(Action<ServiceBusConfiguration> configModification1, Action<ServiceBusConfiguration> configModification2)
        {
            var inputQueueName1 = new EndpointAddress(Guid.NewGuid().ToString());
            var inputQueueName2 = new EndpointAddress(Guid.NewGuid().ToString());

            ServiceBusConfiguration config1 = null;
            ServiceBusConfiguration config2 = null;

            try
            {
                var tracker = new Tracker();
                var unity = new UnityContainer()
                    .RegisterInstance(tracker);

                var bus1 = ServiceBus.Create(c => c
                    .SetServiceLocator(new UnityServiceLocator(unity))
                    .SetInputQueue(inputQueueName1.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName2.GetFriendlyName())
                    .Modify(configModification1)
                    .Out(out config1)
                );

                var bus2 = ServiceBus.Create(c => c
                    .SetServiceLocator(new UnityServiceLocator(unity))
                    .SetInputQueue(inputQueueName2.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName1.GetFriendlyName())
                    .Dispatcher(d => d
                        .AddHandlers(Assembly.GetExecutingAssembly())
                    )
                    .Out(out config2)
                    .Modify(configModification2)
                );

                using (bus1)
                using (bus2)
                {
                    bus1.Run();
                    bus2.Run();

                    var msg = new Message1("Hello", 2010);
                    var msg2 = new Message1("Hello2", 2010);

                    bus1.Send(msg);
                    bus2.Send(msg2);

                    bus2.Wait();
                    bus1.Wait();
                }

                Assert.AreEqual(1, tracker.Handlers.Count);
                Assert.AreEqual(typeof(Message1), tracker.Handlers[0]); 
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