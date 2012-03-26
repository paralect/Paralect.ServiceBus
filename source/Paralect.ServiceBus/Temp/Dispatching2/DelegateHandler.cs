using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Paralect.ServiceBus.Dispatching2
{
    public class DelegateHandler : IHandler
    {
        private readonly Action<Object> _shortAction;
        private readonly Action<Object, IServiceLocator> _fullAction;

        private readonly List<Type> _messageTypes;
        private readonly Object _key;
        private readonly DispatchMode _dispatchMode;

        public DelegateHandler(Action<Object> shortAction, Object key, DispatchMode mode, Type[] messageTypes)
        {
            if (shortAction == null)
                throw new ArgumentNullException("shortAction");

            if (messageTypes.Length == 0)
                throw new Exception("Empty list of subscribed messages for delegate handler. Should be at least one message.");

            _shortAction = shortAction;
            _messageTypes = new List<Type>(messageTypes);
            _dispatchMode = mode;

            // If key wasn't specified, use delegate instance as key.
            _key = key ?? shortAction; 
        }

        public DelegateHandler(Action<Object, IServiceLocator> fullAction, Object key, DispatchMode mode, Type[] messageTypes)
        {
            if (fullAction == null)
                throw new ArgumentNullException("fullAction");

            if (messageTypes.Length == 0)
                throw new Exception("Empty list of subscribed messages for delegate handler. Should be at least one message.");

            _fullAction = fullAction;
            _messageTypes = new List<Type>(messageTypes);
            _dispatchMode = mode;

            // If key wasn't specified, use delegate instance as key.
            _key = key ?? fullAction;
        }

        /// <summary>
        /// Name of the handler. Should show human readable name of handler. Can be not unique.
        /// </summary>
        public string Name
        {
            get { return "Delegate Handler"; }
        }

        /// <summary>
        /// Unique key of the handler. Use this property to uniquily identify this handler.
        /// </summary>
        public Object Key
        {
            get { return _shortAction; }
        }

        /// <summary>
        /// List of types this handler subscribed on
        /// </summary>
        public IEnumerable<Type> Subscriptions
        {
            get { return _messageTypes; }
        }

        /// <summary>
        /// Create executor 
        /// </summary>
        public void Execute(Object message, IServiceLocator serviceLocator)
        {
            if (_shortAction != null)
                _shortAction(message);
            else
                _fullAction(message, serviceLocator);
        }
    }
}