﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.App;
using Paralect.ServiceBus;
using Shared.ClientMessages;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration(AppDomainUnityContext.Current)
                .SetInputQueue("PSB.App2.Input")
                .SetErrorQueue("PSB.App2.Error")
                .AddEndpoint("Shared.ClientMessages", "PSB.App1.Input")
                .AddHandlers(typeof(Program).Assembly);

            var bus = new ServiceBus(config);
            bus.Run();

            Console.WriteLine("Server started. Press enter to send message");

            while (true)
            {
                Console.ReadKey();

                var message = new SayHelloToClientMessage() { Message = "Hello Client!" };
                bus.Send(message);
            }
        }
    }
}
