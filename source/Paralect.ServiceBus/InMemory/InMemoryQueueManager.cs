using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryQueueManager : IQueueManager
    {
        private Dictionary<String, InMemoryQueue> _queues = new Dictionary<string, InMemoryQueue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryQueueManager()
        {
            Translator = new InMemoryMessageTranslator();
        }

        public IMessageTranslator Translator { get; set; }

        public bool Exists(QueueName queueName)
        {
            return _queues.ContainsKey(queueName.GetFriendlyName());
        }

        public void Delete(QueueName queueName)
        {
            _queues.Remove(queueName.GetFriendlyName());
        }

        public IQueue Create(QueueName queueName)
        {
            var queue = new InMemoryQueue(queueName, this);
            _queues[queueName.GetFriendlyName()] = queue;
            return queue;
        }

        public IQueue Open(QueueName queueName)
        {
            if (!_queues.ContainsKey(queueName.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", queueName.GetFriendlyName()));

            return _queues[queueName.GetFriendlyName()];
        }
    }
}