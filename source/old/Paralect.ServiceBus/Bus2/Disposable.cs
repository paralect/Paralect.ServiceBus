using System;

namespace Paralect.ServiceBus.Bus2
{
    public class Disposable : IDisposable
    {
        private Action _dispose;

        public Disposable(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}