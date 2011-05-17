using System;
using System.Messaging;
using System.Linq;
using System.Security.Principal;
using Paralect.ServiceBus.Exceptions;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueue : IQueue
    {
        /// <summary>
        /// Logger instance (In future we should  go away from NLog dependency)
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private readonly QueueName _name;
        private readonly MessageQueue _messageQueue;
        private readonly MsmqQueueManager _manager;

        private String _token = Guid.NewGuid().ToString();

        public QueueName Name
        {
            get { return _name; }
        }

        public IQueueManager Manager
        {
            get { return _manager;  }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqQueue(QueueName name, MessageQueue messageQueue, MsmqQueueManager manager)
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

        public void Send(QueueMessage message)
        {
            if (message.Message == null)
                throw new NullReferenceException("QueueMessage.Message is null");

            if (!(message.Message is System.Messaging.Message))
                throw new ArgumentException("QueueMessage.Message type should be System.Messaging.Message.");

            var msmqMessage = (System.Messaging.Message) message.Message;

            _messageQueue.Send(msmqMessage, 
                _messageQueue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
        }

        /// <summary>
        /// Blocking call
        /// </summary>
        public QueueMessage Receive(TimeSpan timeout)
        {
            try
            {
                // Approach 1:
                // IAsyncResult result = _messageQueue.BeginReceive(timeout);
                // result.AsyncWaitHandle.WaitOne();
                // var message = _messageQueue.EndReceive(result);

                // Approach 2:
                var message = _messageQueue.Receive(timeout);
                var messageType = QueueMessageType.Normal;

                if (message.AppSpecific == (int) QueueMessageType.Shutdown)
                {
                    var guid = System.Text.Encoding.ASCII.GetString(message.Extension);
                    if (String.CompareOrdinal(guid, _token) == 0)
                        messageType = QueueMessageType.Shutdown;
                }

                return new QueueMessage(message, messageType);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    throw new TransportTimeoutException("Timeout when receiving message", mqe);

                throw;
            }
        }

        public void SendStopMessages(Int32 count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                var message = new Message
                {
                    Label = "Shutdown",
                    Extension = System.Text.Encoding.ASCII.GetBytes(_token),
                    AppSpecific = (int)QueueMessageType.Shutdown,
                };

                _messageQueue.Send(message, _messageQueue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
            }
        }

        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}