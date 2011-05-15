using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Paralect.ServiceBus.Dispatcher;
using Paralect.ServiceBus.Msmq;

namespace Paralect.ServiceBus
{
    public class ServiceBus : IDisposable, IBus
    {
        private readonly Configuration _configuration;
        private IQueueTransport _queueTransport;
        private Dispatcher.Dispatcher _dispatcher;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ServiceBus(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Run()
        {
            _dispatcher = new Dispatcher.Dispatcher(
                _configuration.BusContainer,
                _configuration.HandlerRegistry,
                _configuration.MaxRetries);

            _queueTransport = new MsmqQueueTransport(_configuration);
            _queueTransport.QueueObserver.NewMessageArrived += message =>
            {
                _dispatcher.Dispatch(message); ;
            };

            _queueTransport.Start();
        }

        /// <summary>
        /// TODO: multiple messages should be sent at once in one transport message
        /// </summary>
        public void Send(params Object[] messages)
        {
            foreach (var message in messages)
            {
                var queueNames = _configuration.EndpointsMapping.GetQueues(message.GetType());
                foreach (var name in queueNames)
                {
                    _queueTransport.Send(message, name);
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
                _queueTransport.Send(message, _configuration.InputQueue);
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
            if (_queueTransport != null)
                _queueTransport.Stop();
        }


        public void Dispose()
        {
            _logger.Info("Paralect Service [{0}] bus stopped", _configuration.Name);

            if (_queueTransport != null)
                _queueTransport.Stop();
        }
    }
}
