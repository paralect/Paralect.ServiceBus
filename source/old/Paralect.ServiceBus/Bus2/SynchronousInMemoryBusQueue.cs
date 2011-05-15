using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.Bus2
{
    public class SynchronousInMemoryBusQueue<T> : IBusQueue<T>
    {
        private ObserversMananger<T> manager = new ObserversMananger<T>();
        
        private readonly Queue<T> _messages = new Queue<T>();
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return manager.Subscribe(observer);
        }

        public void Enqueue(T message)
        {
            _messages.Enqueue(message);
            Run();
        }

        public void Run()
        {
            if (!manager.ObserversExist)
                return;

            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();

                foreach (var subscription in manager.Observers)
                    subscription.OnNext(message);
            }

        }
    }
}