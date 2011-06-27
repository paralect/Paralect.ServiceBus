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
        public static void AssertTransportMessage(ServiceBusMessage sent, ServiceBusMessage received)
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

        public static void CreateAndOpenQueue(Action<ITransportEndpoint> action, ITransport[] providers = null)
        {
            var name = new TransportEndpointAddress(Guid.NewGuid().ToString());
            
            var managers = providers ?? new ITransport[]
            {
                new MsmqTransport(),
                new InMemoryTransport(),
                new InMemorySynchronousTransport()
            };

            foreach (var manager in managers)
            {
                try
                {
                    manager.CreateEndpoint(name);

                    using (var queue = manager.OpenEndpoint(name))
                    {
                        action(queue);
                    }
                }
                finally
                {
                    manager.DeleteEndpoint(name);
                }
            }
        }

        public static void CreateQueue(Action<ITransportEndpoint> action)
        {
            var name = new TransportEndpointAddress(Guid.NewGuid().ToString());
            
            var managers = new ITransport[]
            {
                new MsmqTransport(),
                new InMemoryTransport(),
                new InMemorySynchronousTransport()
            };

            foreach (var manager in managers)
            {
                try
                {
                    manager.CreateEndpoint(name);
                    var queue = manager.OpenEndpoint(name);
                    action(queue);
                }
                finally
                {
                    manager.DeleteEndpoint(name);
                }
            }
        }
    }
}
