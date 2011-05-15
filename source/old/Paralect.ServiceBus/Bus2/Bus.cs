using System;

namespace Paralect.ServiceBus.Bus2
{
    public class Bus<T> : IBus<T>
    {
        private Boolean _started = false;
        private SynchronousInMemoryBusQueue<T> _queue = new SynchronousInMemoryBusQueue<T>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _queue.Subscribe(observer);
        }

        public void SendLocal(T message)
        {
            _queue.Enqueue(message);
        }

        public void Run()
        {
            _started = true;
            _queue.Run();
        }
    }
}
