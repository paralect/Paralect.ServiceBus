using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Paralect.ServiceBus.InMemory;
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

        public static void CreateAndOpenQueue(Action<IQueue> action)
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            
            var managers = new IQueueManager[]
            {
                new MsmqQueueManager(),
                new InMemoryQueueManager()
            };

            foreach (var manager in managers)
            {
                try
                {
                    manager.Create(name);

                    using (var queue = manager.Open(name))
                    {
                        action(queue);
                    }
                }
                finally
                {
                    manager.Delete(name);
                }
            }
        }

        public static void CreateQueue(Action<IQueue> action)
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            
            var managers = new IQueueManager[]
            {
                new MsmqQueueManager(),
                new InMemoryQueueManager()
            };

            foreach (var manager in managers)
            {
                try
                {
                    var queue = manager.Create(name);
                    action(queue);
                }
                finally
                {
                    manager.Delete(name);
                }
            }
        }
    }
}
