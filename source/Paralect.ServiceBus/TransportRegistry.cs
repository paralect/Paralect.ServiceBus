using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus
{
    /// <summary>
    /// Static 
    /// </summary>
    public static class TransportRegistry
    {
        private static Dictionary<String, ITransport> _map = new Dictionary<String, ITransport>();

        public static void Register(TransportEndpointAddress transportEndpointAddress, ITransport transport)
        {
            if (transport == null)
                return;

            _map[transportEndpointAddress.GetFriendlyName()] = transport;
        }

        public static ITransport GetQueueProvider(TransportEndpointAddress transportEndpointAddress)
        {
            ITransport provider;
            if (!_map.TryGetValue(transportEndpointAddress.GetFriendlyName(), out provider))
                return null;

            return provider;
        }
    }
}