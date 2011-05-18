using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Handlers
{
    public class MessageHandler : IMessageHandler<Message1>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(Message1 message)
        {
            Tracker.Handlers.Add(message.GetType());
        }
    }
}
