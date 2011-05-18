using System;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueueProvider : IQueueProvider
    {
        /// <summary>
        /// Check existence of queue
        /// </summary>
        public Boolean ExistsQueue(QueueName queueName)
        {
            return MessageQueue.Exists(queueName.GetQueueLocalName());
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public IQueue CreateQueue(QueueName queueName)
        {
            var queue = MessageQueue.Create(queueName.GetQueueLocalName(), true); // transactional
            SetupQueue(queue);
            return new MsmqQueue(queueName, queue, this);
        }

        /// <summary>
        /// Delete particular queue
        /// </summary>
        public void DeleteQueue(QueueName queueName)
        {
            MessageQueue.Delete(queueName.GetQueueFormatName());
        }

        /// <summary>
        /// Open queue
        /// </summary>
        public IQueue OpenQueue(QueueName queueName)
        {
            var queue = new MessageQueue(queueName.GetQueueFormatName());
            SetupQueue(queue);
            return new MsmqQueue(queueName, queue, this);
        }

        public IQueueObserver CreateObserver(QueueName queueName)
        {
            return new QueueObserver(this, queueName);
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

        public QueueMessage TranslateToQueueMessage(TransportMessage transportMessage)
        {
            // do not send empty messages
            if (transportMessage.Messages == null || transportMessage.Messages.Length == 0)
                throw new ArgumentException("Transport message is null or empty.");

            Message message = new Message
            {
                Body = transportMessage,
                Formatter = new MsmqMessageFormatter(),
                Label = transportMessage.Messages.First().GetType().FullName
            };

            return new QueueMessage(message);
        }

        public TransportMessage TranslateToTransportMessage(QueueMessage queueMessage)
        {
            if (queueMessage.Message == null)
                throw new NullReferenceException("QueueMessage.Message is null");

            if (!(queueMessage.Message is System.Messaging.Message))
                throw new ArgumentException("QueueMessage.Message type should be System.Messaging.Message.");

            var message = (System.Messaging.Message)queueMessage.Message;
            var transportMessage = (TransportMessage)ReadMessageBody(message);
            return transportMessage;
        }

        private Object ReadMessageBody(Message message)
        {
            try { return message.Body; }
            catch (Exception ex)
            {
                throw new TransportMessageDeserializationException("Error when deserializing transport message", message, ex);
            }
        }
    }
}