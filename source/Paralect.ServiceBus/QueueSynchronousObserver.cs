using System;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus
{
    /*
    public class QueueSynchronousObserver : IQueueObserver
    {
        private readonly IQueue _queue;
        private bool _continue;
        public event Action<QueueMessage, IQueueObserver> MessageReceived;

        public IQueue Queue
        {
            get { return _queue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public QueueSynchronousObserver(IQueue queue)
        {
            _queue = queue;
        }

        public void Dispose()
        {
            
        }

        public void Start()
        {
            _continue = true;

            while (_continue)
            {
                try
                {
                    QueueMessage queueMessage = _queue.Receive(TimeSpan.FromDays(10));

                    if (queueMessage.MessageType == QueueMessageType.Shutdown)
                        break;

                    var received = MessageReceived;
                    if (received != null)
                        received(queueMessage, this);

                }
                catch (TransportTimeoutException)
                {
                    continue;
                }
            }
        }

        public void Wait()
        {
            _queue.SendStopMessages();
            throw new NotImplementedException();
        }
    }*/
}