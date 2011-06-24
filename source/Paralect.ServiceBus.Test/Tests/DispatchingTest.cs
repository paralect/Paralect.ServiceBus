using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Test.Messages;
using UnityServiceLocator = Paralect.ServiceLocator.Unity.UnityServiceLocator;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class DispatchingTest
    {
        [Test]
        public void Simple()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetServiceLocator(new UnityServiceLocator(unity))
                .SetMaxRetries(1)
                .AddHandlers(typeof(DispatchingTest).Assembly)
            );

            var message = new SimpleMessage1();

            dispatcher.Dispatch(message);

            Assert.AreEqual(tracker.Handlers.Count, 1);
            Assert.AreEqual(tracker.Handlers[0], message.GetType());
        }
    }
}