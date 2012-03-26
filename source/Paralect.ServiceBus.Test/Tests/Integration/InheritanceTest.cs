using System;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching2;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Integration
{
    [TestFixture]
    public class InheritanceTest
    {
        [Test]
        public void interface_inheritance()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<IBaseMessage>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Handlers.Count, Is.EqualTo(1));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Int32)));
        }

        [Test]
        public void interface_inheritance_order()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<IBaseMessage>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(String)))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Handlers.Count, Is.EqualTo(2));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Int32)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(String)));
        }

        [Test]
        public void interface_inheritance_swap_order()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(String)))
                .RegisterHandler<IBaseMessage>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Handlers.Count, Is.EqualTo(2));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(String)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(Int32)));
        }

        
    }
}