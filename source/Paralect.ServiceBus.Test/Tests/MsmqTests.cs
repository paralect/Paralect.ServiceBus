using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using System.Messaging;
using Paralect.ServiceBus.Serialization;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class MsmqTests
    {
        public void TestMethod()
        {
            var queueName = @".\private$\MyTestQueue";

            if (!MessageQueue.Exists(queueName))
            {
                MessageQueue.Create(queueName, true); // transactional!!!
            }

            var queue = new MessageQueue(queueName);

            var msg = new SimpleMessage3() { Id = Guid.NewGuid(), Text = "Muaha!" };

            SendMessage(msg, queue);

            var message = ReceiveMessage(queue);

        }

        public void SendMessage(Object message, MessageQueue queue)
        {
            var mqMessage = new Message();
            mqMessage.Formatter = new MessageFormatter();
            mqMessage.Body = message;

            queue.Send(mqMessage);
        }

        public Object ReceiveMessage(MessageQueue queue)
        {
            queue.Formatter = new MessageFormatter();
            var mqMessage = queue.Receive();

            return mqMessage.Body;
        }
    }
}
