using System;
using System.Threading;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class QueueObserverTest
    {
        [Test]
        public void ObserveTest()
        {
            Helper.CreateAndOpenQueue(queue =>
            {
                var messageCount = 0;
                var transportMessage = new ServiceBusMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                using(var observer = queue.Transport.CreateObserver(queue.Name))
                {
                    observer.MessageReceived += (message, ob) =>
                    {
                        messageCount++;
                        Helper.AssertTransportMessage(transportMessage,
                            queue.Transport.TranslateToServiceBusMessage(message));
                    };

                    observer.Start();
                    queue.Send(queue.Transport.TranslateToTransportMessage(transportMessage));
                    queue.Send(queue.Transport.TranslateToTransportMessage(transportMessage));
                    queue.Send(queue.Transport.TranslateToTransportMessage(transportMessage));

                    observer.Wait();
                }

                Assert.AreEqual(messageCount, 3);
            });
        }

        [Test]
        public void TestIt()
        {
            Helper.CreateAndOpenQueue(queue =>
            {
                var transportMessage = new ServiceBusMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                queue.Send(queue.Transport.TranslateToTransportMessage(transportMessage));

                var queue2 = queue.Transport.OpenEndpoint(queue.Name);
                queue2.Send(queue.Transport.TranslateToTransportMessage(transportMessage));
            });
        }
    }
}