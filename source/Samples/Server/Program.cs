using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus;
using Shared.ClientMessages;
using Paralect.ServiceBus.Dispatching;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(AppDomainUnityServiceLocator.Current)
                .MsmqTransport()
                .SetInputQueue("PSB.App2.Input")
                .SetErrorQueue("PSB.App2.Error")
                .AddEndpoint("Shared.ClientMessages", "PSB.App1.Input")
                .Dispatcher(d => d
                    .AddHandlers(typeof(Program).Assembly)
                )
            );

            Console.WriteLine("Server started. Press enter to send message");

            while (true)
            {
                Console.ReadKey();

                var message = new SayHelloToClientMessage() { Message = "Hello Client!" };
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
