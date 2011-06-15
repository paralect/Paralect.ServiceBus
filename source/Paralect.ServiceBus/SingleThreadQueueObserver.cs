using System;
using System.Messaging;
using System.Threading;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class SingleThreadQueueObserver : IQueueObserver
    {
        private readonly IQueueProvider _provider;
        private readonly QueueName _queueName;
        private readonly string _threadName;
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private Thread _observerThread;
        private Boolean _continue;

        public event Action<IQueueObserver> ObserverStarted;
        public event Action<IQueueObserver> ObserverStopped;
        public event Action<QueueMessage, IQueueObserver> MessageReceived;

        private String _shutdownMessageId = Guid.NewGuid().ToString();

        public IQueueProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SingleThreadQueueObserver(IQueueProvider queueProvider, QueueName queueName, String threadName = null)
        {
            _provider = queueProvider;
            _queueName = queueName;
            _threadName = threadName;
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(QueueObserverThread)
            {
                Name = _threadName ?? String.Format("Transport Observer thread for queue {0}", _queueName.GetFriendlyName()),
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
                var started = ObserverStarted;
                if (started != null)
                    started(this);

                _log.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", "_configuration.Name", _queueName.GetFriendlyName());
                Observe();
            });
        }

        private void Observe()
        {
            try
            {
                var queue = _provider.OpenQueue(_queueName);

                while (_continue)
                {
                    try
                    {
                        QueueMessage queueMessage = queue.Receive(TimeSpan.FromDays(10));
                        
                        if (queueMessage.MessageType == QueueMessageType.Shutdown)
                        {
                            if (queueMessage.MessageId == _shutdownMessageId)
                                break;

                            continue;
                        }

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
            _provider
                .OpenQueue(_queueName)
                .Send(new QueueMessage(null, _shutdownMessageId, "Shutdown", QueueMessageType.Shutdown));
        }
    }
}