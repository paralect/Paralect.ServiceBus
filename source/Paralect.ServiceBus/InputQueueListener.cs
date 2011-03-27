using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using Paralect.ServiceBus.Dispatcher;

namespace Paralect.ServiceBus
{
    public class InputQueueListener
    {
        private readonly Configuration _configuration;
        private readonly Dispatcher.Dispatcher _dispatcher;
        private Boolean _continue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InputQueueListener(Configuration configuration, Dispatcher.Dispatcher dispatcher)
        {
            _configuration = configuration;
            _dispatcher = dispatcher;
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
            using (var queue = new MessageQueue(_configuration.InputQueue.GetQueueFormatName()))
            {
                queue.Formatter = new MessageFormatter();

                while (_continue)
                {
                    var transaction = new MessageQueueTransaction();

                    try
                    {
                        transaction.Begin();

                        var message = queue.Receive(new TimeSpan(0, 0, 2), transaction);
                        var obj = message.Body;

                        _dispatcher.Dispatch(obj);

                        transaction.Commit();
                    }
                    catch (MessageQueueException mqe)
                    {
                        if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                            continue;
                    }
                    catch (HandlerException handlerException)
                    {
                        var message = handlerException.MessageObject;
                        SendToErrorQueue(message, transaction);
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        transaction.Abort();
                    }

                    // handle obj
                }
            }            
        }

        private void SendToErrorQueue(object message, MessageQueueTransaction transaction)
        {
            // Open the queue.
            using (var queue = new MessageQueue(_configuration.ErrorQueue.GetQueueLocalName()))
            {
                // Set the formatter to JSON.
                queue.Formatter = new MessageFormatter();

                // Create a message.
                Message myMessage = new Message(message, new MessageFormatter());
                myMessage.Label = "Error message";

                // Send the message.
                queue.Send(myMessage, transaction);
            }
        }
    }
}
