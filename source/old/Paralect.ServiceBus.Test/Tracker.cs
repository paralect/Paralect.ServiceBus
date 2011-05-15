using System;
using System.Collections.Generic;

namespace Paralect.ServiceBus.Test
{
    public class Tracker
    {
        public List<Type> Handlers = new List<Type>();
        public List<Type> Interceptors = new List<Type>();
    }
}
