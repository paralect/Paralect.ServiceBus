using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Shared.ServerMessages;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(AppDomainUnityServiceLocator.Current)
                .MsmqTransport()
                .SetInputQueue("PSB.App1.Input")
                .SetErrorQueue("PSB.App1.Error")
                .AddEndpoint("Shared.ServerMessages", "PSB.App2.Input")
                .Dispatcher(d => d
                    .AddHandlers(typeof(Program).Assembly)
                )
            );

            Console.WriteLine("Client started. Press enter to send messages.");

            while (true)
            {
                Console.ReadKey();

                var message = new SayHelloToServerMessage() { Message = "Hello Server!" };
                bus.Send(message);
            }
        }
    }

    public class AppDomainUnityServiceLocator
    {
        private static Object _lock = new Object();

        private static IServiceLocator _current;

        public static IServiceLocator Current
        {
            get
            {
                IServiceLocator unity = _current;

                if (unity == null)
                {
                    lock (_lock)
                    {
                        if (unity == null)
                        {
                            unity = new Paralect.ServiceLocator.Unity.UnityServiceLocator(new UnityContainer());

                            _current = unity;
                        }
                    }
                }

                return unity;
            }
        }
    }
}
