using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Paralect.ServiceBus.Dispatching2;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests.Dispatcher2
{
    [TestFixture]
    public class Dispatcher2Test
    {
        [Test]
        public void Test2()
        {
/*            IHandler handler1 = new DelegateHandler<Message1>(m => { Console.WriteLine(m.Name); });
            IHandler handler2 = new DelegateHandler<Message2>(m => { Console.WriteLine(m.Height); });

            IHandlerRegistryBuilder builder = new HandlerRegistryBuilder();
            builder.Register(handler1);
            builder.Register(handler2);

            IHandlerRegistry registry = builder.BuildHandlerRegistry();

            var handlers = registry.GetHandlers(typeof(Message1));
            Assert.That(handlers.Count(), Is.EqualTo(1));

            var handlers2 = registry.GetHandlers(typeof(Message2));
            Assert.That(handlers2.Count(), Is.EqualTo(1));*/
        }

        [Test]
        public void Construction()
        {
            /*
            var tracker = new Tracker();
            var unity = new UnityContainer()
                .RegisterInstance(tracker);

            Action<Message1> handler = message => { Console.WriteLine(message.Name); };

            var dispatcher = Dispatcher.Create(d => d
                .SetUnityContainer(unity)
                .RegisterHandler(handler)
                .InsureHandlingOrder<HandlerRegistry, HandlerRegistry, HandlerRegistry>()
                .InsureMessageHandlingOrder<Message1, HandlerRegistry, HandlerRegistry, HandlerRegistry>()
                .RegisterHandler<Message1>(m => { Console.WriteLine(m.Name); })
                .RegisterHandler<Message2>(m => { Console.WriteLine(m.Height); })
                .RegisterAssemblyHandlers<HandlerRegistry>()
                .RegisterHandler(m => { Console.WriteLine(m); }, new [] { typeof(Message1), typeof(Message2), typeof(Message2) })
            );

            dispatcher.Dispatch(new Message1 { Name = "2012 year" });
            */
        }

        public void DelegateHandlerTest()
        {
/*            IHandler handler = new DelegateHandler<Message1>(m => { Console.WriteLine(m.Name); });
            IHandlerExecutor executor = handler.CreateExecutor();
            executor.Execute(new Message1 { Name = "Muhahahaha!" });*/
        }
    }
}
