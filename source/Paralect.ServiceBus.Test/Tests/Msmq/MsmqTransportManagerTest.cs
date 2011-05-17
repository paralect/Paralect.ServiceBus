using System;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;

namespace Paralect.ServiceBus.Test.Tests.Msmq
{
    [TestFixture]
    public class MsmqTransportManagerTest
    {
        [Test]
        public void JustCreation()
        {
            Helper.CreateQueue(queue =>
            {
                // nothing here
            });
        }

        [Test]
        public void ExistCheck()
        {
            Helper.CreateQueue(queue =>
            {
                var exists = queue.Manager.Exists(queue.Name);
                Assert.AreEqual(exists, true);
            });
        }

        [Test]
        public void NotExistCheck()
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            var manager = new MsmqQueueManager();
            var exists = manager.Exists(name);
            Assert.AreEqual(exists, false);
        }


    }
}