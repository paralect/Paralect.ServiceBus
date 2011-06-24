using System;
using System.Messaging;
using System.Linq;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqEndpoint : IEndpoint
    {
        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly EndpointAddress _name;
        private readonly MessageQueue _messageQueue;
        private readonly MsmqEndpointProvider _provider;

        public EndpointAddress Name
        {
            get { return _name; }
        }

        public IEndpointProvider Provider
        {
            get { return _provider;  }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqEndpoint(EndpointAddress name, MessageQueue messageQueue, MsmqEndpointProvider provider)
        {
            _name = name;
            _messageQueue = messageQueue;
            _provider = provider;
        }

        /// <summary>
        /// Delete all message in the queue
        /// </summary>
        public void Purge()
        {
            _messageQueue.Purge();
        }

        public void Send(EndpointMessage message)
        {

            Message msmqMessage = null;

            if (message.Message == null)
            {
                msmqMessage = new Message();
            }
            else
            {
                if (!(message.Message is System.Messaging.Message))
                    throw new ArgumentException("QueueMessage.Message type should be System.Messaging.Message.");

                msmqMessage = (System.Messaging.Message)message.Message;
            }

            msmqMessage.Label = message.MessageName;
            msmqMessage.Extension = System.Text.Encoding.ASCII.GetBytes(message.MessageId);
            msmqMessage.AppSpecific = (int) message.MessageType;

            _messageQueue.Send(msmqMessage, 
                _messageQueue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
        }

        /// <summary>
        /// Blocking call
        /// </summary>
        public EndpointMessage Receive(TimeSpan timeout)
        {
            try
            {
                // Approach 1:
                // IAsyncResult result = _messageQueue.BeginReceive(timeout);
                // result.AsyncWaitHandle.WaitOne();
                // var message = _messageQueue.EndReceive(result);

                // Approach 2:
                var message = _messageQueue.Receive(timeout);
                var messageId = System.Text.Encoding.ASCII.GetString(message.Extension);
                var messageName = message.Label;
                var messageType = (EndpointMessageType) message.AppSpecific;

                return new EndpointMessage(message, messageId, messageName, messageType);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TransportTimeoutException("Timeout when receiving message", mqe);

                throw;
            }
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}