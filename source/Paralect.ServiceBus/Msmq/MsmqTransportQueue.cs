using System;
using System.Messaging;
using System.Linq;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqTransportQueue : ITransportQueue
    {
        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly QueueName _name;
        private readonly MessageQueue _messageQueue;
        private readonly MsmqTransportManager _manager;

        public QueueName Name
        {
            get { return _name; }
        }

        public ITransportManager Manager
        {
            get { return _manager;  }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqTransportQueue(QueueName name, MessageQueue messageQueue, MsmqTransportManager manager)
        {
            _name = name;
            _messageQueue = messageQueue;
            _manager = manager;
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
            // do not send empty messages
            if (message.Messages == null || message.Messages.Length == 0)
                return;

            Message myMessage = new Message(message, new MsmqMessageFormatter());
            myMessage.Label = message.Messages.First().GetType().FullName;
            myMessage.ResponseQueue = _messageQueue;
                
            _messageQueue.Send(myMessage, 
                _messageQueue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
        }

        /// <summary>
        /// Blocking call
        /// </summary>
        public TransportMessage Receive(TimeSpan timeout)
        {
            try
            {
                var message = _messageQueue.Receive(timeout);
                var transportMessage = (TransportMessage) ReadMessageBody(message);
                return transportMessage;
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TransportTimeoutException("Timeout when receiving message", mqe);

                throw;
            }
        }

        private Object ReadMessageBody(Message message)
        {
            try
            {
                return message.Body;
            }
            catch (Exception ex)
            {
                throw new TransportMessageDeserializationException("Error when deserializing transport message", ex);
            }
        }
    }
}