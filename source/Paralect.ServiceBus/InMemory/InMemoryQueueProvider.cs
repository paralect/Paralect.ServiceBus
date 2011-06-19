using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.InMemory
{
    public class InMemoryQueueProvider : IQueueProvider
    {
        protected Dictionary<String, IEndpoint> _queues = new Dictionary<string, IEndpoint>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InMemoryQueueProvider()
        {
        }

        public bool ExistsQueue(QueueName queueName)
        {
            return _queues.ContainsKey(queueName.GetFriendlyName());
        }

        public void DeleteQueue(QueueName queueName)
        {
            _queues.Remove(queueName.GetFriendlyName());
        }

        public virtual void CreateQueue(QueueName queueName)
        {
            var queue = new InMemoryEndpoint(queueName, this);
            _queues[queueName.GetFriendlyName()] = queue;
        }

        public void PrepareQueue(QueueName queueName)
        {
            // nothing to do here...
        }

        public IEndpoint OpenQueue(QueueName queueName)
        {
            if (!_queues.ContainsKey(queueName.GetFriendlyName()))
                throw new Exception(String.Format("There is no queue with name {0}.", queueName.GetFriendlyName()));

            return _queues[queueName.GetFriendlyName()];
        }

        public virtual IQueueObserver CreateObserver(QueueName queueName)
        {
            return new SingleThreadQueueObserver(this, queueName);
        }

        public QueueMessage TranslateToQueueMessage(TransportMessage transportMessage)
        {
            return new QueueMessage(transportMessage);
        }

        public TransportMessage TranslateToTransportMessage(QueueMessage queueMessage)
        {
            if (queueMessage.Message == null)
                throw new NullReferenceException("Message is null");

            if (!(queueMessage.Message is TransportMessage))
                throw new ArgumentException("Message should be of type TransportMessage");

            return (TransportMessage)queueMessage.Message;
        }
    }
}