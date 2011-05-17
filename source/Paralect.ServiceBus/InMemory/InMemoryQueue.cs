using System;
using System.Collections.Generic;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryQueue : IQueue
    {
        private BlockingQueue<QueueMessage> _messages = new BlockingQueue<QueueMessage>();

        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly QueueName _name;
        private readonly IQueueManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryQueue(QueueName name, IQueueManager manager)
        {
            _name = name;
            _manager = manager;
        }

        public void Dispose()
        {
            
        }

        public QueueName Name
        {
            get { return _name;  }
        }

        public IQueueManager Manager
        {
            get { return _manager;  }
        }

        public void Purge()
        {
            _messages.Clear();
        }

        public void Send(QueueMessage message)
        {
            _messages.Enqueue(message);
        }

        public QueueMessage Receive(TimeSpan timeout)
        {
            QueueMessage message;
            var result = _messages.TryDequeue(out message, (int) timeout.TotalMilliseconds);

            if (!result)
                throw new TransportTimeoutException("Timeout when receiving message");

            return message;
        }

        public void SendStopMessages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                QueueMessage message = new QueueMessage(null, QueueMessageType.Shutdown);
                _messages.Enqueue(message);
            }
        }
    }
}