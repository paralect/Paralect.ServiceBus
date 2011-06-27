using System;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqTransport : ITransport
    {
        private readonly string _threadName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqTransport(String threadName = null)
        {
            _threadName = threadName;
        }

        /// <summary>
        /// Check existence of queue
        /// </summary>
        public Boolean ExistsEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            return MessageQueue.Exists(transportEndpointAddress.GetLocalName());
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public void CreateEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            var queue = MessageQueue.Create(transportEndpointAddress.GetLocalName(), true); // transactional
            SetupQueue(queue);
        }

        /// <summary>
        /// Create queue
        /// </summary>
        public void PrepareEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            var queue = new MessageQueue(transportEndpointAddress.GetFormatName());
            SetupQueue(queue);
            SetPermissions(queue);
        }

        /// <summary>
        /// Delete particular queue
        /// </summary>
        public void DeleteEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            MessageQueue.Delete(transportEndpointAddress.GetFormatName());
        }

        /// <summary>
        /// Open queue
        /// </summary>
        public ITransportEndpoint OpenEndpoint(TransportEndpointAddress transportEndpointAddress)
        {
            var queue = new MessageQueue(transportEndpointAddress.GetFormatName());
            SetupQueue(queue);
            return new MsmqTransportEndpoint(transportEndpointAddress, queue, this);
        }

        public ITransportEndpointObserver CreateObserver(TransportEndpointAddress transportEndpointAddress)
        {
            return new SingleThreadTransportEndpointObserver(this, transportEndpointAddress, _threadName);
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

        public TransportMessage TranslateToTransportMessage(ServiceBusMessage serviceBusMessage)
        {
            // do not send empty messages
            if (serviceBusMessage.Messages == null || serviceBusMessage.Messages.Length == 0)
                throw new ArgumentException("Transport message is null or empty.");

            Message message = new Message
            {
                Body = serviceBusMessage,
                Formatter = new MsmqMessageFormatter(),
                Label = serviceBusMessage.Messages.First().GetType().FullName
            };

            return new TransportMessage(message);
        }

        public ServiceBusMessage TranslateToServiceBusMessage(TransportMessage transportMessage)
        {
            if (transportMessage.Message == null)
                throw new NullReferenceException("QueueMessage.Message is null");

            if (!(transportMessage.Message is System.Messaging.Message))
                throw new ArgumentException("QueueMessage.Message type should be System.Messaging.Message.");

            var message = (System.Messaging.Message)transportMessage.Message;

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

            if (!(messageObject is ServiceBusMessage))
                throw new TransportMessageDeserializationException(String.Format(
                    "Error when deserializing transport message. Object type is {0} but should be of type TranportMessage", messageObject.GetType().FullName));

            return (ServiceBusMessage) messageObject;
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