using System;
using Paralect.App;
using Paralect.ServiceBus;
using Shared.ServerMessages;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ServiceBusConfiguration(AppDomainUnityContext.Current)
                .MsmqTransport()
                .SetInputQueue("PSB.App1.Input")
                .SetErrorQueue("PSB.App1.Error")
                .AddEndpoint("Shared.ServerMessages", "PSB.App2.Input")
                .AddHandlers(typeof(Program).Assembly);

            var bus = new ServiceBus(config);
            bus.Run();

            Console.WriteLine("Client started. Press enter to send messages.");

            while (true)
            {
                Console.ReadKey();

                var message = new SayHelloToServerMessage() { Message = "Hello Server!" };
                bus.Send(message);
            }
        }
    }
}
