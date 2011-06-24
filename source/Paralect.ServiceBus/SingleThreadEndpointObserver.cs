using System;
using System.Messaging;
using System.Threading;
using Paralect.ServiceBus.Exceptions;
using Paralect.ServiceBus.Utils;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Observer that use one thread 
    /// </summary>
    public class SingleThreadEndpointObserver : IEndpointObserver
    {
        private readonly IEndpointProvider _provider;
        private readonly EndpointAddress _endpointAddress;
        private readonly string _threadName;
        private Thread _observerThread;
        private Boolean _continue;

        public event Action<IEndpointObserver> ObserverStarted;
        public event Action<IEndpointObserver> ObserverStopped;
        public event Action<EndpointMessage, IEndpointObserver> MessageReceived;

        private readonly String _shutdownMessageId = Guid.NewGuid().ToString();

        /// <summary>
        /// Log
        /// </summary>
        private static NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public IEndpointProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SingleThreadEndpointObserver(IEndpointProvider endpointProvider, EndpointAddress endpointAddress, String threadName = null)
        {
            _provider = endpointProvider;
            _endpointAddress = endpointAddress;
            _threadName = threadName;
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(QueueObserverThread)
            {
                Name = _threadName ?? String.Format("Transport Observer thread for queue {0}", _endpointAddress.GetFriendlyName()),
                IsBackground = true,
            };
            _observerThread.Start();            
        }

        protected void QueueObserverThread(object state)
        {
            // Only one instance of ServiceBus should listen to a particular queue
            String mutexName = String.Format("Paralect.ServiceBus.{0}", _endpointAddress.GetFriendlyName());

            MutexFactory.LockByMutex(mutexName, () =>
            {
                var started = ObserverStarted;
                if (started != null)
                    started(this);

                _log.Info("Paralect Service [{0}] bus started and listen to the {1} queue...", "_configuration.Name", _endpointAddress.GetFriendlyName());
                Observe();
            });
        }

        private void Observe()
        {
            try
            {
                var queue = _provider.OpenQueue(_endpointAddress);

                while (_continue)
                {
                    try
                    {
                        EndpointMessage endpointMessage = queue.Receive(TimeSpan.FromDays(10));
                        
                        if (endpointMessage.MessageType == EndpointMessageType.Shutdown)
                        {
                            if (endpointMessage.MessageId == _shutdownMessageId)
                                break;

                            continue;
                        }

                        var received = MessageReceived;
                        if (received != null)
                            received(endpointMessage, this);
                        
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
                .OpenQueue(_endpointAddress)
                .Send(new EndpointMessage(null, _shutdownMessageId, "Shutdown", EndpointMessageType.Shutdown));
        }
    }
}