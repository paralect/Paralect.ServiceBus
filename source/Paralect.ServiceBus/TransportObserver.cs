using System;
using System.Messaging;
using System.Threading;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Messages;

namespace Paralect.ServiceBus
{
    public class TransportObserver
    {
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly ITransportQueue _transportQueue;
        private Thread _observerThread;
        private Boolean _continue;
        public event Action<TransportMessage, TransportObserver> MessageReceived;
        private String _shutdownToken = Guid.NewGuid().ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransportObserver(ITransportQueue transportQueue)
        {
            _transportQueue = transportQueue;
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(QueueObserverThread)
            {
                Name = String.Format("Transport Observer thread for queue {0}", _transportQueue.Name.GetFriendlyName()),
                IsBackground = true,
            };
            _observerThread.Start();            
        }

        public void StopAndWait()
        {
            Stop();
            Wait();
        }

        public void Stop()
        {
            _transportQueue.Send(new TransportMessage(new ShutdownMessage() {Token = _shutdownToken}));
        }

        protected void QueueObserverThread(object state)
        {
            // Only one instance of ServiceBus should listen to a particular queue
            String mutexName = String.Format("Paralect.ServiceBus.{0}", _transportQueue.Name.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                _log.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", "_configuration.Name", _transportQueue.Name.GetFriendlyName());
                Observe();
            });
        }

        public void Wait()
        {
            _observerThread.Join();
        }

        private void Observe()
        {
            try
            {
                while (_continue)
                {
                    try
                    {
                        TransportMessage message = _transportQueue.Receive(TimeSpan.FromDays(10));

                        if (message.Messages[0].GetType() == typeof(ShutdownMessage))
                        {
                            var shutdown = (ShutdownMessage) message.Messages[0];
                            if (shutdown.Token != _shutdownToken)
                                continue;

                            break;
                        }
                            

                        _log.Info("Received message {0} from sender {1}@{2}",
                            message.GetType().FullName,
                            message.SentFromComputerName,
                            message.SentFromQueueName);

                        var received = MessageReceived;
                        if (received != null)
                            received(message, this);
                        
                    }
                    catch (TransportMessageDeserializationException desearilazationException)
                    {
                        throw;
//                        SendToErrorQueue(message, transaction, desearilazationException);
                    }
                    catch (TransportTimeoutException timeoutException)
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
    }
}