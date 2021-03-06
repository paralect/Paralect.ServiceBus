﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using NLog;
using Paralect.ServiceBus.Dispatcher;
using Paralect.ServiceBus.Messages;

namespace Paralect.ServiceBus.Msmq
{
    public class MsmqQueueObserver : IQueueObserver, IDisposable
    {
        public event Action<Object> NewMessageArrived;

        private static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string _serviceBusName;
        private readonly QueueName _inputQueue;
        private readonly QueueName _errorQueue;
        private readonly MsmqQueueTransport _queueTransport;
        private Boolean _continue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MsmqQueueObserver(string serviceBusName, QueueName inputQueue, QueueName errorQueue, MsmqQueueTransport queueTransport)
        {
            _serviceBusName = serviceBusName;
            _inputQueue = inputQueue;
            _errorQueue = errorQueue;
            _queueTransport = queueTransport;
        }

        /// <summary>
        /// Schedule listener to stop on next iteration
        /// </summary>
        public void Stop()
        {
            _continue = false;
        }

        public void Listen()
        {
            try
            {
                using (var queue = new MessageQueue(_inputQueue.GetQueueFormatName()))
                {
                    queue.MessageReadPropertyFilter.ResponseQueue = true;
                    queue.MessageReadPropertyFilter.SourceMachine = true;
                    queue.Formatter = new MsmqMessageFormatter();

                    while (_continue)
                    {
                        var transaction = new MessageQueueTransaction();

                        try
                        {
                            transaction.Begin();

                            Message message = null;

                            try
                            {
                                message = queue.Receive(transaction);
                                var obj = ReadMessageBody(message);

                                if (obj != null && obj is ShutdownBusMessage)
                                {
                                    transaction.Commit();
                                    continue;
                                }

                                _logger.Info("Received message {0} from sender {1}@{2}",
                                    obj.GetType().FullName,
                                    message.ResponseQueue.MachineName,
                                    message.ResponseQueue.QueueName);

                                var arrived = NewMessageArrived;
                                if (arrived != null)
                                    arrived(obj);
                            }
                            catch (HandlerException handlerException)
                            {
                                SendToErrorQueue(message, transaction, handlerException);
                            }
                            catch (DesearilazationException desearilazationException)
                            {
                                SendToErrorQueue(message, transaction, desearilazationException);
                            }

                            transaction.Commit();
                        }
                        catch (MessageQueueException mqe)
                        {
                            if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                                continue;

                            transaction.Abort();
                            throw;
                        }
                        catch (Exception exception)
                        {
                            transaction.Abort();
                            throw;
                        }
                    }
                }  
            }
            catch (System.Threading.ThreadAbortException abortException)
            {
                var wrapper = new Exception(String.Format("Thread listener was aborted in Service Bus [{0}]", _serviceBusName), abortException);
                _logger.FatalException("", wrapper);
            }
            catch (Exception ex)
            {
                var wrapper = new Exception(String.Format("Fatal exception in Service Bus [{0}]", _serviceBusName), ex);
                _logger.FatalException("", wrapper);
                throw wrapper;
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
                throw new DesearilazationException("Error in deserialization of message", ex);
            }
        }

        private void SendToErrorQueue(Message message, MessageQueueTransaction transaction, Exception exception)
        {
            if (message == null)
                return;

            if (exception != null)
                _logger.ErrorException(String.Format("Message {0} was handled maximum number of times and moved to the error queue. ",
                    message.Label), exception);

            // Open the queue.
            using (var queue = new MessageQueue(_errorQueue.GetQueueLocalName()))
            {
                // Set the formatter to JSON.
                queue.Formatter = new MsmqMessageFormatter();
                message.Formatter = new MsmqMessageFormatter();

                // Send the message.
                queue.Send(message, transaction);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
