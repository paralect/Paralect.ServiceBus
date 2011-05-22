using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Test.Messages;

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
            var inputQueueName1 = new QueueName(Guid.NewGuid().ToString());
            var inputQueueName2 = new QueueName(Guid.NewGuid().ToString());

            try
            {
                var unity = new UnityContainer();
                var tracker = new Tracker();
                unity.RegisterInstance(tracker);

                var config1 = new ServiceBusConfiguration()
                    .SetUnityContainer(unity)
                    .SetInputQueue(inputQueueName1.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName2.GetFriendlyName());

                configModification1(config1);

                var config2 = new ServiceBusConfiguration()
                    .SetUnityContainer(unity)
                    .SetInputQueue(inputQueueName2.GetFriendlyName())
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", inputQueueName1.GetFriendlyName())
                    .AddHandlers(Assembly.GetExecutingAssembly());

                configModification2(config2);

                using (var bus1 = new ServiceBus(config1))
                using (var bus2 = new ServiceBus(config2))
                {
                    bus1.Run();
                    bus2.Run();

                    var msg = new Message1("Hello", 2010);
                    var msg2 = new Message1("Hello2", 2010);

                    bus1.Send(msg);
                    bus2.Send(msg2);

                    bus1.Wait();
                    bus2.Wait();
                }

                Assert.AreEqual(1, tracker.Handlers.Count);
                Assert.AreEqual(typeof(Message1), tracker.Handlers[0]); 
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