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

                using(var observer = new QueueObserver(queue.Manager, queue.Name))
                {
                    observer.MessageReceived += (message, ob) =>
                    {
                        messageCount++;
                        Helper.AssertTransportMessage(transportMessage,
                            queue.Manager.Translator.TranslateToTransportMessage(message));
                    };

                    observer.Start();
                    queue.Send(queue.Manager.Translator.TranslateToQueueMessage(transportMessage));
                    queue.Send(queue.Manager.Translator.TranslateToQueueMessage(transportMessage));
                    queue.Send(queue.Manager.Translator.TranslateToQueueMessage(transportMessage));

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

                queue.Send(queue.Manager.Translator.TranslateToQueueMessage(transportMessage));

                var queue2 = queue.Manager.Open(queue.Name);
                queue2.Send(queue.Manager.Translator.TranslateToQueueMessage(transportMessage));
            });
        }
    }
}