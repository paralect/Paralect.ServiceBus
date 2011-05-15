using System;
using System.Threading;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class TransportObserverTest
    {
        [Test]
        public void ObserveTest()
        {
            Helper.CreateAndOpenQueue(queue =>
            {
                var transportMessage = new TransportMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                queue.Send(transportMessage);
                queue.Send(transportMessage);
                queue.Send(transportMessage);

                TransportObserver observer = new TransportObserver(queue);
                var messageCount = 0;
                observer.MessageReceived += (message, ob) =>
                {
                    messageCount++;
                    Helper.AssertTransportMessage(transportMessage, message);
                };

                observer.Start();
                observer.StopAndWait();

                Assert.AreEqual(messageCount, 3);
            });
        }
    }
}