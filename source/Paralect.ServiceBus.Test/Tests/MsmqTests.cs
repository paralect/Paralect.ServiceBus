using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using System.Messaging;
using Paralect.ServiceBus.Serialization;

namespace Paralect.ServiceBus.Test.Tests
{
    public class MessageClass
    {
        public String Text { get; set; }
        public Guid Id { get; set; }
    }

    namespace test
    {
        public class MessageClass
        {
            public String Text { get; set; }
            public Guid Id { get; set; }
            public Int32 Hello { get; set; }
        }
    }

    [TestFixture]
    public class MsmqTests
    {
        [Test]
        public void TestMethod()
        {
            var queueName = @".\private$\MyTestQueue";

            if (!MessageQueue.Exists(queueName))
            {
                MessageQueue.Create(queueName, true); // transactional!!!
            }

            var queue = new MessageQueue(queueName);

            var msg = new MessageClass() { Id = Guid.NewGuid(), Text = "Muaha!" };

            SendMessage(msg, queue);

            var message = ReceiveMessage(queue);

        }
        
        [Test]
        public void TestSend()
        {
            var config = new Configuration()
                .AddEndpoint("Paralect.ServiceBus.Test.Tests", "PSB.App2.Input")
                .SetInputQueue("PSB.App1.Input")
                .SetErrorQueue("PSB.App1.Error");

            var bus = new ServiceBus(config);
            bus.Run();


            var config2 = new Configuration()
                .AddEndpoint("Paralect.ServiceBus.Test.Tests", "PSB.App1.Input")
                .SetInputQueue("PSB.App2.Input")
                .SetErrorQueue("PSB.App2.Error");

            var bus2 = new ServiceBus(config2);
            bus2.Run();


            var msg = new MessageClass
            {
                Id = Guid.NewGuid(),
                Text = "From dddbfdfdgfgusd! Muaha!"
            };

            bus.Send(msg);

            Thread.Sleep(5000);
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
