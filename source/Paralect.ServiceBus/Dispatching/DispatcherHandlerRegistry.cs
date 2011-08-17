using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paralect.ServiceBus.Dispatching
{
    public class DispatcherHandlerRegistry
    {
        private Type _markerInterface;

        public Type MarkerInterface
        {
            get { return _markerInterface; }
            set { _markerInterface = value; }
        }

        /// <summary>
        /// Message type -> List of handlers type
        /// </summary>
        private Dictionary<Type, List<Type>> _subscription = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Message interceptors
        /// </summary>
        private List<Type> _interceptors = new List<Type>();

        /// <summary>
        /// Message interceptors
        /// </summary>
        public List<Type> Interceptors
        {
            get { return _interceptors; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherHandlerRegistry()
        {
            _markerInterface = typeof(IMessageHandler<>);
        }

        /// <summary>
        /// Register all handlers in assembly (you can register handlers that optionally belongs to specified namespaces)
        /// </summary>
        public void Register(Assembly assembly, String[] namespaces)
        {
            var searchTarget = _markerInterface;

            var assemblySubscriptions = assembly
                .GetTypes()
                .Where(t => BelongToNamespaces(t, namespaces))
                .SelectMany(t => t.GetInterfaces()
                                    .Where(i => i.IsGenericType
                                    && (i.GetGenericTypeDefinition() == searchTarget)
                                    && !i.ContainsGenericParameters),
                            (t, i) => new { Key = i.GetGenericArguments()[0], Value = t })
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var key in assemblySubscriptions.Keys)
            {
                var types = assemblySubscriptions[key];

                if (!_subscription.ContainsKey(key))
                    _subscription[key] = new List<Type>();

                foreach (var type in types)
                {
                    // Skip handler already registered for that message type, skip it!
                    if (!_subscription[key].Contains(type))
                        _subscription[key].Add(type);
                }
            }
        }

        public void InsureOrderOfHandlers(List<Type> order)
        {
            if (order.Count <= 1)
                return;

            foreach (var type in _subscription.Keys)
            {
                var handlerTypes = _subscription[type];
                SortInPlace(handlerTypes, order);
            }
        }

        public void SortInPlace(List<Type> list, List<Type> orders)
        {
            if (orders.Count <= 1)
                return;

            list.Sort((type1, type2) =>
            {
                var first = orders.IndexOf(type1);
                var second = orders.IndexOf(type2);

                if (first == -1 && second == -1)
                    return 0;

                if (first == -1 && second != -1)
                    return 1;

                if (first != -1 && second == -1)
                    return -1;

                return (first < second) ? -1 : 1;
            });
        }

        public void AddInterceptor(Type type)
        {
            if (!typeof(IMessageHandlerInterceptor).IsAssignableFrom(type))
                throw new Exception(String.Format("Interceptor {0} must implement IMessageHandlerInterceptor", type.FullName));

            if (_interceptors.Contains(type))
                throw new Exception(String.Format("Interceptor {0} already registered", type.FullName));

            _interceptors.Add(type);
        }

        private Boolean BelongToNamespaces(Type type, String[] namespaces)
        {
            // if no namespaces specified - then type belong to any namespace
            if (namespaces.Length == 0)
                return true;

            foreach (var ns in namespaces)
            {
                if (type.FullName.StartsWith(ns))
                    return true;
            }

            return false;
        }

        public List<Type> GetHandlersType(Type messageType)
        {
            String errorMessage = String.Format("Handler for type {0} doesn't found.", messageType.FullName);

            if (!_subscription.ContainsKey(messageType))
                return new List<Type>();
//                throw new Exception(errorMessage);

            var handlers = _subscription[messageType];

            if (handlers.Count < 1)
                throw new Exception(errorMessage);

            return handlers;
        }
    }
}
