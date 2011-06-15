using System;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemorySynchronousQueue : IQueue, IQueueObserver
    {
        private readonly QueueName _name;
        private readonly IQueueProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemorySynchronousQueue(QueueName name, IQueueProvider provider)
        {
            _name = name;
            _provider = provider;
        }

        public void Dispose()
        {
            
        }

        public QueueName Name
        {
            get { return _name; }
        }

        public event Action<IQueueObserver> ObserverStarted;
        public event Action<IQueueObserver> ObserverStopped;
        public event Action<QueueMessage, IQueueObserver> MessageReceived;

        IQueueProvider IQueueObserver.Provider
        {
            get { return _provider; }
        }

        public void Start()
        {
        }

        public void Wait()
        {
        }

        IQueueProvider IQueue.Provider
        {
            get { return _provider; }
        }

        public void Purge()
        {
        }

        public void Send(QueueMessage message)
        {
            var received = MessageReceived;

            if (received != null)
                received(message, this);
        }

        public QueueMessage Receive(TimeSpan timeout)
        {
            throw new InvalidOperationException("You cannot call Receive() method on Synchronous Queue");
        }
    }
}