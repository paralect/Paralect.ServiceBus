using System;
using System.Messaging;
using System.Security.Principal;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqTransportManager
    {
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
        public MsmqTransportQueue Create(QueueName queueName)
        {
            var queue = MessageQueue.Create(queueName.GetQueueLocalName(), true); // transactional
            SetupQueue(queue);
            return new MsmqTransportQueue(queue);
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
        public MsmqTransportQueue Open(QueueName queueName)
        {
            using (var queue = new MessageQueue(queueName.GetQueueFormatName()))
            {
                SetupQueue(queue);
                return new MsmqTransportQueue(queue);
            }
        }

        private void SetupQueue(MessageQueue queue)
        {
            queue.MessageReadPropertyFilter.ResponseQueue = true;
            queue.MessageReadPropertyFilter.SourceMachine = true;
            queue.Formatter = new MsmqMessageFormatter();

            var userName = WindowsIdentity.GetCurrent().Name;
            MsmqPermissionManager.SetPermissionsForQueue(queue, userName);
        }
    }
}