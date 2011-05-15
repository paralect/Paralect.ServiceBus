using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Paralect.ServiceBus.Dispatcher;
using Paralect.ServiceBus.Messages;

namespace Paralect.ServiceBus.Bus2
{
    public class Message<T>
    {
        public T[] Value { get; set; }
        public Object Transport { get; set; }
    }

    public class MsmqQueue<T> : IObservable<T>, IDisposable
    {
        private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ObserversMananger<T> _manager = new ObserversMananger<T>();
        private readonly QueueName _queueName;
        private Thread _observerThread;
        private Boolean _continue;

        public MsmqQueue(QueueName queueName)
        {
            _queueName = queueName;
            _manager.ObserversChanged += manager => Check();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _manager.Subscribe(observer);
        }

        public void Check()
        {
            if (_manager.Count == 0)
            {
                // shutdown listener
                Stop();
            }
        }

        public void Start()
        {
            _continue = true;
            _observerThread = new Thread(ObserverThread)
            {
                Name = String.Format("MSMQ ({0}) Observer Thread", _queueName.Name),
                IsBackground = true,
            };
            _observerThread.Start();            
        }

        public void Stop()
        {
            _continue = false;
        }

        public void Send(T message)
        {
            // Open the queue.
            using (var queue = new MessageQueue(_queueName.GetQueueFormatName()))
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
                    myMessage.ResponseQueue = new MessageQueue(_queueName.GetQueueFormatName());

                    // Send the message.
                    queue.Send(myMessage, mqt);

                    mqt.Commit();
                }
            }
        }

        public void ObserverThread()
        {
            try
            {
                using (var queue = new MessageQueue(_queueName.GetQueueFormatName()))
                {
                    queue.MessageReadPropertyFilter.ResponseQueue = true;
                    queue.MessageReadPropertyFilter.SourceMachine = true;
                    queue.Formatter = new MsmqMessageFormatter();

                    while (_continue)
                    {
                        try
                        {
                            Message message = queue.Receive();
                            T obj = ReadMessageBody(message);

                            if (obj != null && obj is ShutdownBusMessage)
                            {
                                continue;
                            }

                            _logger.Info("Received message {0} from sender {1}@{2}",
                                obj.GetType().FullName,
                                message.ResponseQueue.MachineName,
                                message.ResponseQueue.QueueName);

                            foreach (var subscription in _manager.Observers)
                                subscription.OnNext(obj);

                        }
                        catch (DesearilazationException desearilazationException)
                        {
                            foreach (var subscription in _manager.Observers)
                                subscription.OnError(desearilazationException);
                        }
                        catch (MessageQueueException mqe)
                        {
                            if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                                continue;

                            throw;
                        }
                    }
                }
            }
            catch (ThreadAbortException abortException)
            {
                var wrapper = new Exception(String.Format("Thread listener was aborted in Service Bus"), abortException);
                _logger.FatalException("", wrapper);
            }
            catch (Exception ex)
            {
                var wrapper = new Exception(String.Format("Fatal exception in Service Bus"), ex);
                _logger.FatalException("", wrapper);
                throw wrapper;
            }            
        }

        private T ReadMessageBody(Message message)
        {
            try
            {
                return (T) message.Body;
            }
            catch (Exception ex)
            {
                throw new DesearilazationException("Error in deserialization of message", ex);
            }
        }


        public void Dispose()
        {
            _manager.UsubscribeAll();
        }
    }
}
