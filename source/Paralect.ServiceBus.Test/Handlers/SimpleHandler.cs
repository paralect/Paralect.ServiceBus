using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Dispatcher;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Handlers
{
    public class SimpleHandler : 
        IMessageHandler<SimpleMessage>,
        IMessageHandler<SimpleMessage2>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(SimpleMessage message)
        {
            Tracker.Handlers.Add(message.GetType());
        }

        public void Handle(SimpleMessage2 message)
        {
            Tracker.Handlers.Add(message.GetType());
        }
    }
}
