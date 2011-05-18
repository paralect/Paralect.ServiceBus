using System;
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
        public void ObserveTest()
        {
            var inputQueue = new QueueName(Guid.NewGuid().ToString());
            var errorQueue = new QueueName(Guid.NewGuid().ToString());

            var unity = new UnityContainer();
            var tracker = new Tracker();
            unity.RegisterInstance(tracker);

            var config1 = new Configuration(unity)
                .MsmqTransport();

            var config2 = new Configuration(unity)
                .MsmqTransport();

            config1.SetInputQueue("PSB.App1.Input")
                .SetErrorQueue("PSB.App1.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App2.Input", config2.QueueProvider);



            config2.SetInputQueue("PSB.App2.Input")
                .SetErrorQueue("PSB.App2.Error")
                .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App1.Input", config1.QueueProvider)
                .AddHandlers(Assembly.GetExecutingAssembly());

            using (var bus1 = new ServiceBus(config1))
            using (var bus2 = new ServiceBus(config2))
            {
                bus1.Run();
                bus2.Run();

                var msg = new Message1("Hello", 2010);

                bus1.Send(msg);

                bus1.Wait();
                bus2.Wait();
            }

            Assert.AreEqual(1, tracker.Handlers.Count);
            Assert.AreEqual(typeof(Message1), tracker.Handlers[0]);

/*
            IQueueProvider provider = new Msmq.MsmqQueueProvider();

            try
            {
                using (var bus = new ServiceBus(provider, inputQueue, errorQueue))
                {
                    bus.Dispatcher.


                    bus.Wait();
                }
            }
            finally
            {
                provider.DeleteQueue(inputQueue);
                provider.DeleteQueue(errorQueue);
            }*/
        }        
    }
}