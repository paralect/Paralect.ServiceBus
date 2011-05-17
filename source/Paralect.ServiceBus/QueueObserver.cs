using System;
using System.Messaging;
using System.Threading;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    public class QueueObserver : IDisposable
    {
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IQueue _queue;
        private Thread _observerThread;
        private Boolean _continue;
        public event Action<QueueMessage, QueueObserver> MessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public QueueObserver(IQueue queue)
        {
            _queue = queue;
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(QueueObserverThread)
            {
                Name = String.Format("Transport Observer thread for queue {0}", _queue.Name.GetFriendlyName()),
                IsBackground = true,
            };
            _observerThread.Start();            
        }

        protected void QueueObserverThread(object state)
        {
            // Only one instance of ServiceBus should listen to a particular queue
            String mutexName = String.Format("Paralect.ServiceBus.{0}", _queue.Name.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                _log.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", "_configuration.Name", _queue.Name.GetFriendlyName());
                Observe();
            });
        }

        private void Observe()
        {
            try
            {
                while (_continue)
                {
                    try
                    {
                        QueueMessage queueMessage = _queue.Receive(TimeSpan.FromDays(10));

                        if (queueMessage.MessageType == QueueMessageType.Shutdown)
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
            _queue.SendStopMessages();
            _observerThread.Join();
        }

        public void Dispose()
        {
            _continue = false;
            _queue.SendStopMessages();
            _observerThread.Join();
        }
    }
}