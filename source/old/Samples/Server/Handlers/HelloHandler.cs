using System;
using Paralect.ServiceBus;
using Shared.ClientMessages;
using Shared.ServerMessages;

namespace Server.Handlers
{
    public class HelloHandler : IMessageHandler<SayHelloToServerMessage>
    {
        public void Handle(SayHelloToServerMessage message)
        {
            Console.WriteLine(message.Message);
        }
    }
}