using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;

namespace Paralect.ServiceBus.Test
{
    public class Helper
    {
        public static void AssertTransportMessage(TransportMessage sent, TransportMessage received)
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


        public static void CreateAndOpenQueue(Action<MsmqTransportQueue> action)
        {
            CreateAndOpenQueue((queue, manager) =>
            {
                action(queue);
            });
        }

        public static void CreateAndOpenQueue(Action<MsmqTransportQueue, MsmqTransportManager> action)
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            var manager = new MsmqTransportManager();

            try
            {
                manager.Create(name);
                var queue = manager.Open(name);

                action((MsmqTransportQueue)queue, manager);
            }
            finally
            {
                manager.Delete(name);
            }
        }

        public static void CreateQueue(Action<QueueName, MsmqTransportManager> action)
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            var manager = new MsmqTransportManager();

            try
            {
                manager.Create(name);
                action(name, manager);
            }
            finally
            {
                manager.Delete(name);
            }
        }
    }
}
