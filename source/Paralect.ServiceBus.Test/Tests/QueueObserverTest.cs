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
                var transportMessage = new TransportMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                using(var observer = queue.Provider.CreateObserver(queue.Name))
                {
                    observer.MessageReceived += (message, ob) =>
                    {
                        messageCount++;
                        Helper.AssertTransportMessage(transportMessage,
                            queue.Provider.TranslateToTransportMessage(message));
                    };

                    observer.Start();
                    queue.Send(queue.Provider.TranslateToQueueMessage(transportMessage));
                    queue.Send(queue.Provider.TranslateToQueueMessage(transportMessage));
                    queue.Send(queue.Provider.TranslateToQueueMessage(transportMessage));

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
                var transportMessage = new TransportMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                queue.Send(queue.Provider.TranslateToQueueMessage(transportMessage));

                var queue2 = queue.Provider.OpenQueue(queue.Name);
                queue2.Send(queue.Provider.TranslateToQueueMessage(transportMessage));
            });
        }
    }
}