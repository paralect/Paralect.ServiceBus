using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Paralect.ServiceBus.Bus2;
using System.Reactive.Linq;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Tests
{
    public static class Extensions
    {
        public static IObservable<T> MoveToErrorQueue<T>(this MsmqQueue<T> observable, MsmqQueue<T> errorQueue)
        {
            return Observable.Create<T>(ob =>
            {
                return observable.Subscribe(
                i => ob.OnNext(i),
                x => {},
                () => ob.OnCompleted());
            });            
        }

        public static IObservable<T> StopAfter<T>(this IObservable<T> observable, int messageCount)
        {
            var number = 0;

            return Observable.Create<T>(ob =>
            {
                return observable.Subscribe(
                i =>
                {
                    ob.OnNext(i);
                    number++;

                    if (number >= messageCount)
                    {
                        ob.OnCompleted();
                    }
                },
                x =>
                {
                    ob.OnError(x);
                },
                () => ob.OnCompleted());
            });              
        }
    }

    [TestFixture]
    public class MsmqQueueTest
    {
        [Test]
        public void Test()
        {
            var errorQueue = new MsmqQueue<Object>(new QueueName("PSB.App1.Error"));
            var queue = new MsmqQueue<Object>(new QueueName("PSB.App1.Input"));
            queue.Send(new SimpleMessage3() { Id = Guid.NewGuid(), Text = "lyapsya2!"} );

            var waitHandle = new AutoResetEvent(false);

            queue.MoveToErrorQueue(errorQueue)
                .StopAfter(1)
                .Subscribe(obj =>
                {
                    Console.WriteLine("Message received");
                }, () => waitHandle.Set());

            queue.Start();

            Console.WriteLine("After subscribe");

            waitHandle.WaitOne(5000);

            Thread.Sleep(3000);
        }
    }
}
