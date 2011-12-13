using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceBus.Test.Handlers;
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

            Assert.AreEqual(tracker.Handlers.Count, 3);
            Assert.AreEqual(tracker.Handlers[0], message.GetType());
        }        
        
        [Test]
        public void OrderTestOne()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetServiceLocator(new UnityServiceLocator(unity))
                .SetMaxRetries(1)
                .AddHandlers(typeof(DispatchingTest).Assembly)
                .SetOrder(typeof(FirstHandler), typeof(SecondHandler))
            );

            var message = new SimpleMessage1();

            dispatcher.Dispatch(message);

            Assert.AreEqual(tracker.Handlers.Count, 3);

            var firstHandler = tracker.Handlers.IndexOf(typeof(FirstHandler));
            var secondHandler = tracker.Handlers.IndexOf(typeof(SecondHandler));

            Assert.AreEqual(firstHandler < secondHandler, true);
        }        

        [Test]
        public void OrderTestTwo()
        {
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            var dispatcher = Dispatcher.Create(d => d
                .SetServiceLocator(new UnityServiceLocator(unity))
                .SetMaxRetries(1)
                .AddHandlers(typeof(DispatchingTest).Assembly)
                .SetOrder(typeof(SecondHandler), typeof(SecondHandler))
            );

            var message = new SimpleMessage1();

            dispatcher.Dispatch(message);

            Assert.AreEqual(tracker.Handlers.Count, 3);

            var firstHandler = tracker.Handlers.IndexOf(typeof(FirstHandler));
            var secondHandler = tracker.Handlers.IndexOf(typeof(SecondHandler));

            Assert.AreEqual(firstHandler > secondHandler, true);
        }
    }
}