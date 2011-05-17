using System;
using System.Messaging;
using System.Threading;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class QueueObserver : IQueueObserver
    {
        private readonly IQueueManager _queueManager;
        private readonly QueueName _queueName;
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private Thread _observerThread;
        private Boolean _continue;
        public event Action<QueueMessage, IQueueObserver> MessageReceived;

        private String _shutdownMessageId = Guid.NewGuid().ToString();

        public IQueueManager QueueManager
        {
            get { return _queueManager; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public QueueObserver(IQueueManager queueManager, QueueName queueName)
        {
            _queueManager = queueManager;
            _queueName = queueName;
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(QueueObserverThread)
            {
                Name = String.Format("Transport Observer thread for queue {0}", _queueName.GetFriendlyName()),
                IsBackground = true,
            };
            _observerThread.Start();            
        }

        protected void QueueObserverThread(object state)
        {
            // Only one instance of ServiceBus should listen to a particular queue
            String mutexName = String.Format("Paralect.ServiceBus.{0}", _queueName.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                _log.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", "_configuration.Name", _queueName.GetFriendlyName());
                Observe();
            });
        }

        private void Observe()
        {
            try
            {
                var queue = _queueManager.Open(_queueName);

                while (_continue)
                {
                    try
                    {
                        QueueMessage queueMessage = queue.Receive(TimeSpan.FromDays(10));

                        // Break on shutdown message
                        if (queueMessage.MessageType == QueueMessageType.Shutdown &&
                            queueMessage.MessageId == _shutdownMessageId)
                            break;

                        var received = MessageReceived;
                        if (received != null)
                            received(queueMessage, this);
                        
                    }
                    catch (TransportTimeoutException)
                    {
                        continue;
                    }
                }
                
            }
            catch (ThreadAbortException abortException)
            {
                var wrapper = new Exception(String.Format("Thread listener was aborted in Service Bus [{0}]", "_serviceBusName"), abortException);
                _log.FatalException("", wrapper);
            }
            catch (Exception ex)
            {
                var wrapper = new Exception(String.Format("Fatal exception in Service Bus [{0}]", "_serviceBusName"), ex);
                _log.FatalException("", wrapper);
                throw wrapper;
            }
        }

        public void Wait()
        {
            SendStopMessages();
            _observerThread.Join();
        }

        public void Dispose()
        {
            _continue = false;
            SendStopMessages();
            _observerThread.Join();
        }

        private void SendStopMessages()
        {
            _queueManager
                .Open(_queueName)
                .Send(new QueueMessage(null, _shutdownMessageId, "Shutdown", QueueMessageType.Shutdown));
        }
    }
}