using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace Paralect.ServiceBus.Dispatching2
{
    public static class DispatcherConfigurationDelegateExtensions
    {
        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
        // 1. General registration (Action with IServiceLocator)
        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  

        /// <summary>
        /// Register named general delegate <see cref="Action{Object}"/> subscribed on list of messages, with specified DispatchMode
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="uniqueName">Unique name that can be used to identify this handler</param>
        /// <param name="mode">Dispatching mode. Default value is DispatchMode.InterfaceDescendants</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object, IServiceLocator> action, String uniqueName, DispatchMode mode, params Type[] messageTypes)
        {
            IHandler handler = new DelegateHandler(action, uniqueName, mode, messageTypes);
            configuration.Builder.Register(handler);
            return configuration;
        }

        /// <summary>
        /// Register named general delegate <see cref="Action{Object}"/> subscribed on list of messages
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="uniqueName">Unique name that can be used to identify this handler</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object, IServiceLocator> action, String uniqueName, params Type[] messageTypes)
        {
            return RegisterHandler(configuration, action, uniqueName, DispatchMode.InterfaceDescendants, messageTypes);
        }

        /// <summary>
        /// Register delegate as handler, with list of message types this handler subscribed to.
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object, IServiceLocator> action, params Type[] messageTypes)
        {
            return RegisterHandler(configuration, action, null, messageTypes);
        }

        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
        // 1. General registration (Action without IServiceLocator)
        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  

        /// <summary>
        /// Register named general delegate <see cref="Action{Object}"/> subscribed on list of messages, with specified DispatchMode
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="uniqueName">Unique name that can be used to identify this handler</param>
        /// <param name="mode">Dispatching mode. Default value is DispatchMode.InterfaceDescendants</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object> action, String uniqueName, DispatchMode mode, params Type[] messageTypes)
        {
            IHandler handler = new DelegateHandler(action, uniqueName, mode, messageTypes);
            configuration.Builder.Register(handler);
            return configuration;
        }


        /// <summary>
        /// Register named general delegate <see cref="Action{Object}"/> subscribed on list of messages
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="uniqueName">Unique name that can be used to identify this handler</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object> action, String uniqueName, params Type[] messageTypes)
        {
            return RegisterHandler(configuration, action, uniqueName, DispatchMode.InterfaceDescendants, messageTypes);
        }

        /// <summary>
        /// Register delegate as handler, with list of message types this handler subscribed to.
        /// </summary>
        /// <param name="action">Delegate instance</param>
        /// <param name="messageTypes">List of message types this handler subscribed on</param>
        public static DispatcherConfiguration RegisterHandler(this DispatcherConfiguration configuration, Action<Object> action, params Type[] messageTypes)
        {
            return RegisterHandler(configuration, action, null, messageTypes);
        }

        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
        // 3. Generic registration (Action with IServiceLocator)
        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  

        /// <summary>
        /// Register named delegate that subscribed to single message TMessage with specified DispatchMode.
        /// </summary>
        /// <typeparam name="TMessage">Message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage>(this DispatcherConfiguration configuration, Action<TMessage> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, m => action((TMessage)m), uniqueName, dispatchMode, new[] { typeof(TMessage) });
        }

        /// <summary>
        /// Register handler that subscribed to two messages. 
        /// </summary>
        /// <typeparam name="TMessage1">First message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage2">Second message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage1, TMessage2>(this DispatcherConfiguration configuration, Action<Object> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, action, uniqueName, dispatchMode, new[] { typeof(TMessage1), typeof(TMessage2) });
        }

        /// <summary>
        /// Register handler that subscribed to three messages. 
        /// </summary>
        /// <typeparam name="TMessage1">First message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage2">Second message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage3">Third message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage1, TMessage2, TMessage3>(this DispatcherConfiguration configuration, Action<Object> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, action, uniqueName, dispatchMode, new[] { typeof(TMessage1), typeof(TMessage2), typeof(TMessage3) });
        }


        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  
        // 3. Generic registration (Action with IServiceLocator)
        // -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  

        /// <summary>
        /// Register named delegate that subscribed to single message TMessage with specified DispatchMode.
        /// </summary>
        /// <typeparam name="TMessage">Message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage>(this DispatcherConfiguration configuration, Action<TMessage, IServiceLocator> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, (m, s) => action((TMessage)m, s), uniqueName, dispatchMode, new[] { typeof(TMessage) });
        }

        /// <summary>
        /// Register handler that subscribed to two messages. 
        /// </summary>
        /// <typeparam name="TMessage1">First message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage2">Second message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage1, TMessage2>(this DispatcherConfiguration configuration, Action<Object, IServiceLocator> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, action, uniqueName, dispatchMode, new[] { typeof(TMessage1), typeof(TMessage2) });
        }

        /// <summary>
        /// Register handler that subscribed to three messages. 
        /// </summary>
        /// <typeparam name="TMessage1">First message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage2">Second message type this handler subscribed to</typeparam>
        /// <typeparam name="TMessage3">Third message type this handler subscribed to</typeparam>
        /// <param name="action">Delegate instance</param>
        public static DispatcherConfiguration RegisterHandler<TMessage1, TMessage2, TMessage3>(this DispatcherConfiguration configuration, Action<Object, IServiceLocator> action, String uniqueName = null, DispatchMode dispatchMode = DispatchMode.InterfaceDescendants)
        {
            return RegisterHandler(configuration, action, uniqueName, dispatchMode, new[] { typeof(TMessage1), typeof(TMessage2), typeof(TMessage3) });
        }
    }
}
