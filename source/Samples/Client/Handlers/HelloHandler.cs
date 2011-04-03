using System;
using Paralect.ServiceBus;
using Shared.ClientMessages;

namespace Client.Handlers
{
    public class HelloHandler : IMessageHandler<SayHelloToClientMessage>
    {
        public void Handle(SayHelloToClientMessage message)
        {
            Console.WriteLine(message.Message);
        }
    }
}