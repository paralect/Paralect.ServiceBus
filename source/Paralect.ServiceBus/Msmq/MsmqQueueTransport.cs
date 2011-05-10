using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueueTransport : IQueueTransport
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Thread _observerThread;
        private MsmqQueueObserver _queueObserver;

        private Configuration _configuration;

        public IQueueObserver QueueObserver
        {
            get { return _queueObserver; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqQueueTransport(Configuration configuration)
        {
            _configuration = configuration;
            _queueObserver = new MsmqQueueObserver(_configuration.Name, _configuration.InputQueue, _configuration.ErrorQueue);
        }

        public void Start()
        {
            CheckAvailabilityOfQueue(_configuration.InputQueue);
            CheckAvailabilityOfQueue(_configuration.ErrorQueue);

            _observerThread = new Thread(QueueObserverThread)
            {
                Name = String.Format("Paralect Service Bus {0} Worker Thread #{1}", _configuration.Name, 1),
                IsBackground = true,
            };
            _observerThread.Start(_queueObserver);
        }

        public void Stop()
        {
            if (_queueObserver != null)
                _queueObserver.Stop();
        }

        protected void QueueObserverThread(object state)
        {
            var msmqQueueObserver = (MsmqQueueObserver)state;

            // Only one instance of ServiceBus should listen to a particular queue
            String mutexName = String.Format("Paralect.ServiceBus.{0}", _configuration.InputQueue.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                _logger.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", _configuration.Name, _configuration.InputQueue.GetFriendlyName());
                msmqQueueObserver.Listen();
            });
        }

        public void CheckAvailabilityOfQueue(QueueName queue)
        {
            // Only one instance of ServiceBus should create queue
            String mutexName = String.Format("Paralect.ServiceBus.QueueChecker.{0}", _configuration.InputQueue.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                var userName = WindowsIdentity.GetCurrent().Name;

                if (!MessageQueue.Exists(queue.GetQueueLocalName()))
                {
                    MessageQueue.Create(queue.GetQueueLocalName(), true); // transactional
                    MsmqPermissionManager.SetPermissionsForQueue(queue.GetQueueLocalName(), userName, _configuration.Name);
                }
                else
                {
                    MsmqPermissionManager.SetPermissionsForQueue(queue.GetQueueLocalName(), userName, _configuration.Name);
                }
            });
        }

        public void Send(object message, QueueName queueName)
        {
            // Open the queue.
            using (var queue = new MessageQueue(queueName.GetQueueFormatName()))
            {
                SendIternal(message, queue);
            }
        }

        private void SendIternal(Object message, MessageQueue queue)
        {
            // Set the formatter to JSON.
            queue.Formatter = new MsmqMessageFormatter();

            // Since we're using a transactional queue, make a transaction.
            using (MessageQueueTransaction mqt = new MessageQueueTransaction())
            {
                mqt.Begin();

                // Create a simple text message.
                Message myMessage = new Message(message, new MsmqMessageFormatter());
                myMessage.Label = message.GetType().FullName;
                myMessage.ResponseQueue = new MessageQueue(_configuration.InputQueue.GetQueueFormatName());

                // Send the message.
                queue.Send(myMessage, mqt);

                mqt.Commit();
            }
        }
    }
}
