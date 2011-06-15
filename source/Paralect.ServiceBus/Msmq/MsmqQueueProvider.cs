using System;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueueProvider : IQueueProvider
    {
        private readonly string _threadName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqQueueProvider(String threadName = null)
        {
            _threadName = threadName;
        }

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
        public void CreateQueue(QueueName queueName)
        {
            var queue = MessageQueue.Create(queueName.GetQueueLocalName(), true); // transactional
            SetupQueue(queue);
//            SetPermissions(queue);
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public void PrepareQueue(QueueName queueName)
        {
            var queue = new MessageQueue(queueName.GetQueueFormatName());
            SetupQueue(queue);
            SetPermissions(queue);
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
            return new SingleThreadQueueObserver(this, queueName, _threadName);
        }

        private void SetupQueue(MessageQueue queue)
        {
            queue.MessageReadPropertyFilter.ResponseQueue = true;
            queue.MessageReadPropertyFilter.SourceMachine = true;
            queue.MessageReadPropertyFilter.AppSpecific = true;
            queue.MessageReadPropertyFilter.Extension = true;
            queue.Formatter = new MsmqMessageFormatter();
        }

        private void SetPermissions(MessageQueue queue)
        {
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

            Object messageObject;
            try
            {
                messageObject = message.Body;
            }
            catch (Exception ex)
            {
                throw new TransportMessageDeserializationException("Error when deserializing transport message", message, ex);
            }

            if (messageObject == null)
                throw new TransportMessageDeserializationException("MessageBody is null - cannot translate to TransportMessage");

            if (!(messageObject is TransportMessage))
                throw new TransportMessageDeserializationException(String.Format(
                    "Error when deserializing transport message. Object type is {0} but should be of type TranportMessage", messageObject.GetType().FullName));

            return (TransportMessage) messageObject;
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