using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable, IBus
    {
        private readonly Configuration _configuration;
        private static object receivingLock = new object();
        private Thread _workerThread;
        private InputQueueListener _inputQueueListener;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly string LocalAdministratorsGroupName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).ToString();
        private static readonly string LocalEveryoneGroupName = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();
        private static readonly string LocalAnonymousLogonName = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();


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

            _logger.Info("Paralect Service bus started...");
        }

        protected void BackgroundThread(object state)
        {


            var dispatcher = new Dispatcher.Dispatcher(
                _configuration.BusContainer, 
                _configuration.HandlerRegistry,
                _configuration.MaxRetries);
            
            _inputQueueListener = new InputQueueListener(_configuration.InputQueue, _configuration.ErrorQueue, dispatcher);
            _inputQueueListener.Listen();
        }

        public void CheckAvailabilityOfQueue(QueueName queue)
        {
            var userName = WindowsIdentity.GetCurrent().Name;

            if (!MessageQueue.Exists(queue.GetQueueLocalName()))
            {
                MessageQueue.Create(queue.GetQueueLocalName(), true); // transactional
                SetPermissionsForQueue(queue.GetQueueLocalName(), userName);
            }
            else
            {
                SetPermissionsForQueue(queue.GetQueueLocalName(), userName);
            }
        }

        /// <summary>
        /// Sets default permissions for queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="account"></param>
        public static void SetPermissionsForQueue(string queue, string account)
        {
            var q = new MessageQueue(queue);

            try
            {
                q.SetPermissions(LocalAdministratorsGroupName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                q.SetPermissions(LocalEveryoneGroupName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
                q.SetPermissions(LocalAnonymousLogonName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

                q.SetPermissions(account, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
                q.SetPermissions(account, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
                q.SetPermissions(account, MessageQueueAccessRights.PeekMessage, AccessControlEntryType.Allow);
            }
            catch (Exception ex)
            {
                var message = String.Format("Access to MSMQ queue '{0}' is denied. Please set permission for this queue to be accessable for '{1}' account.", queue, account);
                
                _logger.ErrorException(message, ex);
                throw new Exception(message, ex);
            }
        } 

        /// <summary>
        /// TODO: multiple messages should be sent at once in one transport message
        /// </summary>
        /// <param name="messages"></param>
        public void Send(params Object[] messages)
        {
            foreach (var message in messages)
            {
                var queueName = _configuration.EndpointsMapping.GetQueues(message.GetType());

                foreach (var name in queueName)
                {
                    // Open the queue.
                    using (var queue = new MessageQueue(name.GetQueueFormatName()))
                    {
                        SendIternal(message, queue);
                    }
                }                
            }
        }

        /// <summary>
        /// TODO: multiple messages should be sent at once in one transport message
        /// </summary>
        public void SendLocal(params Object[] messages)
        {
            foreach (var message in messages)
            {
                // Open the queue.
                using (var queue = new MessageQueue(_configuration.InputQueue.GetQueueLocalName()))
                {
                    SendIternal(message, queue);
                }
            }
        }

        private void SendIternal(Object message, MessageQueue queue)
        {
            // Set the formatter to JSON.
            queue.Formatter = new MessageFormatter();

            // Since we're using a transactional queue, make a transaction.
            using (MessageQueueTransaction mqt = new MessageQueueTransaction())
            {
                mqt.Begin();

                // Create a simple text message.
                Message myMessage = new Message(message, new MessageFormatter());
                myMessage.Label = message.GetType().FullName;
                myMessage.ResponseQueue = new MessageQueue(_configuration.InputQueue.GetQueueFormatName());

                // Send the message.
                queue.Send(myMessage, mqt);

                mqt.Commit();
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
            if (_inputQueueListener != null)
                _inputQueueListener.Stop();
        }


        public void Dispose()
        {
            if (_inputQueueListener != null)
                _inputQueueListener.Stop();
        }
    }
}
