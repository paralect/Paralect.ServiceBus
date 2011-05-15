using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public interface IBus
    {
        void Send(params Object[] message);
        void SendLocal(params Object[] message);
        void Publish(Object message);
    }
}
