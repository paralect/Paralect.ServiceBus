using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Paralect.ServiceBus.Test.Tests
{
    public static class EXT
    {
        private static IObservable<int> CreateFastObservable(int iterations)
        {
            return Observable.Create<int>(observer =>
            {
                new Thread(_ =>
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        observer.OnNext(i);
                    }
                    observer.OnCompleted();
                }).Start();
                return () => { };
            });
        }    
    }

    public class SObserver<T> : IObserver<T>
    {
        private Action<T> onNext;
        private Action<Exception> onError;
        private Action onCompleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public static SObserver<T> Create(Action<T> onNext)
        {
            return new SObserver<T>(onNext, e => { }, () => { });
        }

        public static SObserver<T> Create(Action<T> onNext, Action onCompleted)
        {
            return new SObserver<T>(onNext, e => { }, onCompleted);
        }

        public void OnNext(T value)
        {
            onNext(value);
        }

        public void OnError(Exception error)
        {
            onError(error);
        }

        public void OnCompleted()
        {
            onCompleted();
        }
    }

    public static class RXRXRXR
    {
        public static IObservable<T> LimitTo5Events<T>(this IObservable<T> obs)
        {
            var counter = 0;

            return Observable.Create<T>(ob =>
            {
                return obs.Subscribe(SObserver<T>.Create(i =>
                {
                    counter++;
                    ob.OnNext((T)(Object)counter);
//                    ob.OnNext(i);
                },
                () => ob.OnCompleted()));
            });
        }
    }

    [TestFixture]
    public class ObservableTest
    {


        [Test]
        public void Do()
        {
            Observable.Return(45);


            var collection = Observable.Range(10, 50, Scheduler.CurrentThread).LimitTo5Events();
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            collection.Subscribe(i =>
            {
                Console.Write(Thread.CurrentThread.ManagedThreadId);
                Console.Write(" ");
                Console.WriteLine(i);
            },
            () => Console.Write("DONE!!!"));
            
        }
    }
}
