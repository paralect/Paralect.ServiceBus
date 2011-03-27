using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Handlers
{
    public class MessageHandler : IMessageHandler<SimpleMessage3>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(SimpleMessage3 message)
        {
            Tracker.Handlers.Add(message.GetType());

            throw new Exception();
        }
    }
}
