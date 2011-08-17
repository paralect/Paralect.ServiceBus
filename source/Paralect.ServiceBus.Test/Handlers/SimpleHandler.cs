using Microsoft.Practices.Unity;
using Paralect.ServiceBus.Test.Messages;

namespace Paralect.ServiceBus.Test.Handlers
{
    public class SimpleHandler : 
        IMessageHandler<SimpleMessage1>,
        IMessageHandler<SimpleMessage2>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(SimpleMessage1 message1)
        {
            Tracker.Handlers.Add(message1.GetType());
        }

        public void Handle(SimpleMessage2 message)
        {
            Tracker.Handlers.Add(message.GetType());
        }
    }    
    
    public class FirstHandler : 
        IMessageHandler<SimpleMessage1>,
        IMessageHandler<SimpleMessage2>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(SimpleMessage1 message1)
        {
            Tracker.Handlers.Add(GetType());
        }

        public void Handle(SimpleMessage2 message)
        {
            Tracker.Handlers.Add(GetType());
        }
    }

    public class SecondHandler : 
        IMessageHandler<SimpleMessage1>,
        IMessageHandler<SimpleMessage2>
    {
        [Dependency]
        public Tracker Tracker { get; set; }

        public void Handle(SimpleMessage1 message1)
        {
            Tracker.Handlers.Add(GetType());
        }

        public void Handle(SimpleMessage2 message)
        {
            Tracker.Handlers.Add(GetType());
        }
    }
}
