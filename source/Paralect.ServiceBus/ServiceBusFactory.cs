using System;

namespace Paralect.ServiceBus
{
    public class ServiceBusFactory
    {
        public static IBus Create(Func<ServiceBusConfiguration, ServiceBusConfiguration> configurationAction)
        {
            var config = new ServiceBusConfiguration();
            configurationAction(config);

            return new ServiceBus(config);
        }
    }
}