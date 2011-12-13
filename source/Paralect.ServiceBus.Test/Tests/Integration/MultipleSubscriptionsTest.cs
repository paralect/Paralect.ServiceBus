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
    public class MultipleSubscriptionsTest
    {
        [Test]
        public void subscribe_on_two_messages_generic_without_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1, Message2>(m => { m1++; })
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message2());
            Assert.That(m1, Is.EqualTo(1));
        }

        [Test]
        public void subscribe_on_three_messages_generic_without_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1, Message2, Message3>(m => { m1++; })
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message2());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message3());
            Assert.That(m1, Is.EqualTo(1));
        }

        [Test]
        public void subscribe_on_two_messages_generic_with_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1, Message2>((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); })
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));

            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));

            dispatcher.Dispatch(new Message3());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));
        }

        [Test]
        public void subscribe_on_three_messages_generic_with_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1, Message2, Message3>((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); })
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));

            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));

            dispatcher.Dispatch(new Message3());
            Assert.That(tracker.Messages.Count, Is.EqualTo(3));
        }

        [Test]
        public void subscribe_on_two_messages_not_generic_without_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler(m => { m1++; }, typeof(Message1), typeof(Message2))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message2());
            Assert.That(m1, Is.EqualTo(1));
        }

        [Test]
        public void subscribe_on_three_messages_not_generic_without_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            int m1 = 0;

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler(m => { m1++; }, typeof(Message1), typeof(Message2), typeof(Message3))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message2());
            Assert.That(m1, Is.EqualTo(1));

            m1 = 0;
            dispatcher.Dispatch(new Message3());
            Assert.That(m1, Is.EqualTo(1));
        }

        [Test]
        public void subscribe_on_two_messages_not_generic_with_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); }, typeof(Message1), typeof(Message2))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));

            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));

            dispatcher.Dispatch(new Message3());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));
        }

        [Test]
        public void subscribe_on_three_messages_not_generic_with_locator()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler((m, s) => { s.GetInstance<Tracker>().Messages.Add(m.GetType()); }, typeof(Message1), typeof(Message2), typeof(Message3))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Messages.Count, Is.EqualTo(1));

            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Messages.Count, Is.EqualTo(2));

            dispatcher.Dispatch(new Message3());
            Assert.That(tracker.Messages.Count, Is.EqualTo(3));
        }
    }

}
