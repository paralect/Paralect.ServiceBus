using System;
using System.Messaging;
using System.Linq;
using System.Security.Principal;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqTransportQueue : ITransportQueue
    {
        private readonly MessageQueue _messageQueue;

        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqTransportQueue(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
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

        public TransportMessage Receive(TimeSpan timeout)
        {
            var message = _messageQueue.Receive(timeout);
            var transportMessage = (TransportMessage) message.Body;
            return transportMessage;
        }
    }
}