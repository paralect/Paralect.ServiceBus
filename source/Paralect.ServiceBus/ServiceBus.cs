using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable
    {
        private readonly Configuration _configuration;
        private static object receivingLock = new object();
        private Thread _workerThread;
        private Boolean _continue = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBus(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Run()
        {
            CheckAvailabilityOfQueue(_configuration.InputQueue);
            CheckAvailabilityOfQueue(_configuration.ErrorQueue);

            _workerThread = new Thread(BackgroundThread)
            {
                Name = "Paralect Service Bus Worker Thread #" + 1,
                IsBackground = true,
            };
            _workerThread.Start();
        }

        protected void BackgroundThread(object state)
        {
            var dispatcher = new Dispatcher.Dispatcher(_configuration.BusContainer, _configuration.HandlerRegistry);

            using (var queue = new MessageQueue(_configuration.InputQueue.GetQueueFormatName()))
            {
                queue.Formatter = new MessageFormatter();

                while(_continue)
                {
                    try
                    {
                        var message = queue.Receive(new TimeSpan(0, 0, 2));
                        var obj = message.Body;

                        dispatcher.Dispatch(obj);


                    }
                    catch (MessageQueueException mqe)
                    {
                        if (mqe.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                            continue;
                    }

                    // handle obj
                }
            }
        }

        public void CheckAvailabilityOfQueue(QueueName queue)
        {
            if (!MessageQueue.Exists(queue.GetQueueLocalName()))
            {
                MessageQueue.Create(queue.GetQueueLocalName(), true); // transactional
            }            
        }

        public void Send(Object message)
        {
            var queueName = _configuration.EndpointsMapping.GetQueues(message.GetType());

            foreach (var name in queueName)
            {
                // Open the queue.
                using (var queue = new MessageQueue(name.GetQueueFormatName()))
                {
                    // Set the formatter to JSON.
                    queue.Formatter = new MessageFormatter();

                    // Since we're using a transactional queue, make a transaction.
                    using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                    {
                        mqt.Begin();

                        // Create a simple text message.
                        Message myMessage = new Message(message, new MessageFormatter());
                        myMessage.Label = "First Message";

                        // Send the message.
                        queue.Send(myMessage, mqt);

                        mqt.Commit();
                    }
                }                        
            }
        }

        /// <summary>
        /// For now Publish() behaves as Send(). Subscription not yet implemented.
        /// </summary>
        public void Publish(Object message)
        {
            Send(message);
        }

        ~ServiceBus()
        {
            _continue = false;
        }


        public void Dispose()
        {
            _continue = false;
        }
    }
}
