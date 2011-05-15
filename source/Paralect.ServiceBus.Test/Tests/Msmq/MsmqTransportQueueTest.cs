using System;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Msmq
{
    [TestFixture]
    public class MsmqTransportQueueTest
    {
        [Test]
        public void PurgingWhenJustCreated()
        {
            CreateAndOpenQueue(queue =>
            {
                queue.Purge();
            });
        }        
        
        [Test]
        public void SendAndReceiveSingleMessageQueue()
        {
            CreateAndOpenQueue((queue, manager) =>
            {
                var transportMessage = new TransportMessage(new object[]
                {
                    new Message1("MessageName", 2011)
                });

                queue.Send(transportMessage);
                var receivedMessage = queue.Receive(TimeSpan.FromSeconds(5));

                AssertTransportMessage(transportMessage, receivedMessage);

            });
        }      
  
        [Test]
        public void SendAndReceiveMultipleMessageQueue()
        {
            CreateAndOpenQueue((queue, manager) =>
            {
                var transportMessage = new TransportMessage(new object[]
                {
                    new Message1("MessageName1", 2011),
                    new Message2("MessageName2", 2012),
                    new Message1("MessageName3", 2013),
                    new Message2("MessageName4", 2014),
                    new Message2("MessageName5", 2015),
                });

                queue.Send(transportMessage);
                var receivedMessage = queue.Receive(TimeSpan.FromSeconds(5));

                AssertTransportMessage(transportMessage, receivedMessage);

            });
        }

        #region Helpers

        public void AssertTransportMessage(TransportMessage sent, TransportMessage received)
        {
            Assert.AreEqual(sent.Messages.Length, received.Messages.Length);

            for (int i = 0; i < sent.Messages.Length; i++)
            {
                var sentMessage = sent.Messages[i];
                var receivedMessage = received.Messages[i];

                Assert.AreEqual(sentMessage.GetType(), receivedMessage.GetType());
                ObjectComparer.AreObjectsEqual(sentMessage, receivedMessage);
            }
        }

        private void CreateAndOpenQueue(Action<MsmqTransportQueue> action)
        {
            CreateAndOpenQueue((queue, manager) =>
            {
                action(queue);
            });
        }        
        
        private void CreateAndOpenQueue(Action<MsmqTransportQueue, MsmqTransportManager> action)
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            var manager = new MsmqTransportManager();

            try
            {
                manager.Create(name);
                var queue = manager.Open(name);

                action(queue, manager);
            }
            finally
            {
                manager.Delete(name);
            }
        }

        #endregion 

    }
}