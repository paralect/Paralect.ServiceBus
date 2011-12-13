using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Paralect.ServiceBus.Dispatching2
{
    public static class DispatcherConfigurationAssemblyExtensions
    {
        public static DispatcherConfiguration RegisterAssemblyHandlers(this DispatcherConfiguration configuration, Assembly assembly) { return configuration; }

        public static DispatcherConfiguration UnregisterAssemblyHandlers(this DispatcherConfiguration configuration, Assembly assembly) { return configuration; }

        /// <summary>
        /// Register assembly's handlers. Assembly located by TAssemblyType.
        /// </summary>
        /// <typeparam name="TAssemblyType"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DispatcherConfiguration RegisterAssemblyHandlers<TAssemblyType>(this DispatcherConfiguration configuration) { return configuration; }

        /// <summary>
        /// TAssemblyType can be different, then was used in RegisterAssemblyHandlers
        /// </summary>
        public static DispatcherConfiguration UnregisterAsseblyHandlers<TAssemblyType>(this DispatcherConfiguration configuration) { return configuration; }

    }
}
