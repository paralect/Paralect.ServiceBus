using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class ConcurrentTest
    {
        [Test]
        public void Test()
        {
            Parallel.For(0, 15, (i, state) =>
            {
                var unity = new UnityContainer();
                var tracker = new Tracker();
                unity.RegisterInstance(tracker);

                var config = new Configuration(unity)
                    .SetName("PS " + i)
                    .SetInputQueue("PSB.App1.Input")
                    .SetErrorQueue("PSB.App1.Error")
                    .AddEndpoint("Paralect.ServiceBus.Test.Messages", "PSB.App2.Input");

                using (var bus = new ServiceBus(config))
                {
                    bus.Run();
                    Thread.Sleep(10);
                }
            });
        }
    }
}
