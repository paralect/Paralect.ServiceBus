using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Static 
    /// </summary>
    public static class EndpointProviderRegistry
    {
        private static Dictionary<String, IEndpointProvider> _map = new Dictionary<String, IEndpointProvider>();

        public static void Register(EndpointAddress endpointAddress, IEndpointProvider endpointProvider)
        {
            if (endpointProvider == null)
                return;

            _map[endpointAddress.GetFriendlyName()] = endpointProvider;
        }

        public static IEndpointProvider GetQueueProvider(EndpointAddress endpointAddress)
        {
            IEndpointProvider provider;
            if (!_map.TryGetValue(endpointAddress.GetFriendlyName(), out provider))
                return null;

            return provider;
        }
    }
}