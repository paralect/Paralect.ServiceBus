using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public interface IMessageHandler<TMessage>
    {
        void Handle(TMessage message);
    }
}
