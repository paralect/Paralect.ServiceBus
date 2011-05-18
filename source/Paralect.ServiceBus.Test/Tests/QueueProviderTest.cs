using System;
using NUnit.Framework;
using Paralect.ServiceBus.Msmq;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class QueueProviderTest
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
                var exists = queue.Provider.ExistsQueue(queue.Name);
                Assert.AreEqual(exists, true);
            });
        }

        [Test]
        public void NotExistCheck()
        {
            var name = new QueueName(Guid.NewGuid().ToString());
            var manager = new MsmqQueueProvider();
            var exists = manager.ExistsQueue(name);
            Assert.AreEqual(exists, false);
        }


    }
}