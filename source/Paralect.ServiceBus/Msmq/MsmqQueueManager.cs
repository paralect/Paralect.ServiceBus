using System;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueueManager : IQueueManager
    {
        public IMessageTranslator Translator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqQueueManager()
        {
            Translator = new MsmqMessageTranslator();
        }

        /// <summary>
        /// Check existence of queue
        /// </summary>
        public Boolean Exists(QueueName queueName)
        {
            return MessageQueue.Exists(queueName.GetQueueLocalName());
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public IQueue Create(QueueName queueName)
        {
            var queue = MessageQueue.Create(queueName.GetQueueLocalName(), true); // transactional
            SetupQueue(queue);
            return new MsmqQueue(queueName, queue, this);
        }

        /// <summary>
        /// Delete particular queue
        /// </summary>
        public void Delete(QueueName queueName)
        {
            MessageQueue.Delete(queueName.GetQueueFormatName());
        }

        /// <summary>
        /// Open queue
        /// </summary>
        public IQueue Open(QueueName queueName)
        {
            var queue = new MessageQueue(queueName.GetQueueFormatName());
            SetupQueue(queue);
            return new MsmqQueue(queueName, queue, this);
        }

        private void SetupQueue(MessageQueue queue)
        {
            queue.MessageReadPropertyFilter.ResponseQueue = true;
            queue.MessageReadPropertyFilter.SourceMachine = true;
            queue.MessageReadPropertyFilter.AppSpecific = true;
            queue.MessageReadPropertyFilter.Extension = true;
            queue.Formatter = new MsmqMessageFormatter();

            var userName = WindowsIdentity.GetCurrent().Name;
            MsmqPermissionManager.SetPermissionsForQueue(queue, userName);
        }
    }
}