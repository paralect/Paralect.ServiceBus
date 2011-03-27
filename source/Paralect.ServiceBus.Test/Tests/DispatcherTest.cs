using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatcher;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture()]
    public class DispatcherTest
    {
        [Test]
        public void Simple()
        {
            var unity = new UnityContainer();
            var tracker = new Tracker();
            unity.RegisterInstance(tracker);

            var registrator = new HandlerRegistry();
            registrator.Register(Assembly.GetExecutingAssembly(), new string[] {});

            var dispatcher = new Dispatcher.Dispatcher(unity, registrator, 1);

            var message = new SimpleMessage1();
            dispatcher.Dispatch(message);

            var message2 = new SimpleMessage2();
            dispatcher.Dispatch(message2);

            Assert.AreEqual(2, tracker.Handlers.Count);
            Assert.AreEqual(typeof(SimpleMessage1), tracker.Handlers[0]);
            Assert.AreEqual(typeof(SimpleMessage2), tracker.Handlers[1]);
        }
    }
}
