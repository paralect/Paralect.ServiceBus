﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paralect.ServiceBus.Dispatcher
{
    public class HandlerRegistry
    {
        private Type _markerInterface;

        public Type MarkerInterface
        {
            get { return _markerInterface; }
            set { _markerInterface = value; }
        }

        /// <summary>
        /// Message type -> Handler type
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
        public HandlerRegistry()
        {
            _markerInterface = typeof(IMessageHandler<>);
        }

        /// <summary>
        /// 
        /// </summary>
 /*       public void RegisterAssemblies(Assembly[] assemblies, String[] namespaces)
        {
            var searchTarget = _markerInterface;

            var dict = assemblies.SelectMany(a => a.GetTypes().Where(t => BelongToNamespaces(t, namespaces)))
                .SelectMany(t => t.GetInterfaces()
                                    .Where(i => i.IsGenericType
                                    && (i.GetGenericTypeDefinition() == searchTarget)
                                    && !i.ContainsGenericParameters),
                            (t, i) => new { Key = i.GetGenericArguments()[0], Value = t })
                .GroupBy(x => x.Key, x => x.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            _subscription = dict; 
        }*/

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

                _subscription[key].AddRange(types);
            }
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
