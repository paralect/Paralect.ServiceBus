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
                var exists = queue.Transport.ExistsEndpoint(queue.Name);
                Assert.AreEqual(exists, true);
            });
        }

        [Test]
        public void NotExistCheck()
        {
            var name = new TransportEndpointAddress(Guid.NewGuid().ToString());
            var manager = new MsmqTransport();
            var exists = manager.ExistsEndpoint(name);
            Assert.AreEqual(exists, false);
        }


    }
}