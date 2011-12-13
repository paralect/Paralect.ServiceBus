using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching2;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Integration
{
    [TestFixture]
    public class DispatcherTests
    {
        [Test]
        public void message_dispatching()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;
            int m2 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>(m => { m1++; })
                .RegisterHandler<Message2>(m => { m2++; })
            );

            dispatcher.Dispatch(new Message1());

            Assert.That(m1, Is.EqualTo(1));
            Assert.That(m2, Is.EqualTo(0));
            m1 = m2 = 0;

            dispatcher.Dispatch(new Message2());

            Assert.That(m1, Is.EqualTo(0));
            Assert.That(m2, Is.EqualTo(1));
        }

        [Test]
        public void message_dispatching_not_generic()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;
            int m2 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler(m => { m1++; }, typeof(Message1))
                .RegisterHandler(m => { m2++; }, typeof(Message2))
            );

            dispatcher.Dispatch(new Message1());

            Assert.That(m1, Is.EqualTo(1));
            Assert.That(m2, Is.EqualTo(0));
            m1 = m2 = 0;

            dispatcher.Dispatch(new Message2());

            Assert.That(m1, Is.EqualTo(0));
            Assert.That(m2, Is.EqualTo(1));
        }

        [Test]
        public void message_dispatching_with_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); })
                .RegisterHandler<Message2>((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); })
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));
            Assert.That(tracker.Messages[0], Is.EqualTo(typeof(Message1)));

            tracker.Reset();
            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));
            Assert.That(tracker.Messages[0], Is.EqualTo(typeof(Message2)));

            tracker.Reset();
            dispatcher.Dispatch(new Message3());
            Assert.That(tracker.Messages.Count, Is.EqualTo(0));
        }

        [Test]
        public void dispatch_should_not_throw_exception_if_no_handlers_registered_at_all()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(0));
        }

        [Test]
        public void dispatch_should_not_throw_exception_if_no_handlers_registered_for_particular_message()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message2>(m => {})
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(0));
        }
    }
}
