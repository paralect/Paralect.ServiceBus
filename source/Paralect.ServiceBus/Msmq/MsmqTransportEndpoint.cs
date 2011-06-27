using System;
using System.Messaging;
using System.Linq;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqTransportEndpoint : ITransportEndpoint
    {
        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly TransportEndpointAddress _name;
        private readonly MessageQueue _messageQueue;
        private readonly MsmqTransport _transport;

        public TransportEndpointAddress Name
        {
            get { return _name; }
        }

        public ITransport Transport
        {
            get { return _transport;  }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqTransportEndpoint(TransportEndpointAddress name, MessageQueue messageQueue, MsmqTransport transport)
        {
            _name = name;
            _messageQueue = messageQueue;
            _transport = transport;
        }

        /// <summary>
        /// Delete all message in the queue
        /// </summary>
        public void Purge()
        {
            _messageQueue.Purge();
        }

        public void Send(TransportMessage message)
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
        public TransportMessage Receive(TimeSpan timeout)
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
                var messageType = (TransportMessageType) message.AppSpecific;

                return new TransportMessage(message, messageId, messageName, messageType);
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