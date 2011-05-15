using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus.Bus2
{
    /// <summary>
    /// NOT thread safe realization
    /// </summary>
    public class ObserversMananger<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public event Action<ObserversMananger<T>> ObserversChanged;

        public IEnumerable<IObserver<T>> Observers
        {
            get { return _observers; }
        }

        public Int32 Count
        {
            get { return _observers.Count; }
        }

        public Boolean ObserversExist
        {
            get { return _observers.Count > 0;  }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);

            if (ObserversChanged != null)
                ObserversChanged(this);

            return new Disposable(() =>
            {
                _observers.Remove(observer);

                if (ObserversChanged != null)
                    ObserversChanged(this);
            });
        }

        public void UsubscribeAll()
        {
            _observers.Clear();

            if (ObserversChanged != null)
                ObserversChanged(this);
        }
    }
}
