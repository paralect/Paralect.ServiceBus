using System;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching2;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Integration
{
    [TestFixture]
    public class BasicOrderTest
    {
        [Test]
        public void basic_order_for_two_handlers()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(String)))
            );

            dispatcher.Dispatch(new Message1());

            Assert.That(tracker.Handlers.Count, Is.EqualTo(2));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Int32)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(String)));
        }
         
        [Test]
        public void basic_order_for_four_handlers()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Byte)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int16)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int64)))
            );

            dispatcher.Dispatch(new Message1());

            Assert.That(tracker.Handlers.Count, Is.EqualTo(4));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Byte)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(Int16)));
            Assert.That(tracker.Handlers[2], Is.EqualTo(typeof(Int32)));
            Assert.That(tracker.Handlers[3], Is.EqualTo(typeof(Int64)));
        }

        [Test]
        public void basic_order_for_eight_handlers_interleaved()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Byte)))
                .RegisterHandler<Message2>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Byte)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int16)))
                .RegisterHandler<Message2>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int16)))
                .RegisterHandler<Message2>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
                .RegisterHandler<Message2>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int64)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int32)))
                .RegisterHandler<Message1>((m, s) => s.GetInstance<Tracker>().Handlers.Add(typeof(Int64)))
            );

            dispatcher.Dispatch(new Message1());
            Assert.That(tracker.Handlers.Count, Is.EqualTo(4));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Byte)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(Int16)));
            Assert.That(tracker.Handlers[2], Is.EqualTo(typeof(Int32)));
            Assert.That(tracker.Handlers[3], Is.EqualTo(typeof(Int64)));

            tracker.Reset();
            dispatcher.Dispatch(new Message2());
            Assert.That(tracker.Handlers.Count, Is.EqualTo(4));
            Assert.That(tracker.Handlers[0], Is.EqualTo(typeof(Byte)));
            Assert.That(tracker.Handlers[1], Is.EqualTo(typeof(Int16)));
            Assert.That(tracker.Handlers[2], Is.EqualTo(typeof(Int32)));
            Assert.That(tracker.Handlers[3], Is.EqualTo(typeof(Int64)));

        }
    }
}