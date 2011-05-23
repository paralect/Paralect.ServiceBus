using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Static 
    /// </summary>
    public static class QueueProviderRegistry
    {
        private static Dictionary<String, IQueueProvider> _map = new Dictionary<String, IQueueProvider>();

        public static void Register(QueueName queueName, IQueueProvider queueProvider)
        {
            if (queueProvider == null)
                return;

            _map[queueName.GetFriendlyName()] = queueProvider;
        }

        public static IQueueProvider GetQueueProvider(QueueName queueName)
        {
            IQueueProvider provider;
            if (!_map.TryGetValue(queueName.GetFriendlyName(), out provider))
                return null;

            return provider;
        }
    }
}