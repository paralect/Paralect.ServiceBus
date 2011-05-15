using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using NUnit.Framework;
using System.Reactive.Linq;
using Paralect.ServiceBus.Bus2;

namespace Paralect.ServiceBus.Test.Tests
{
    [TestFixture]
    public class BusTest
    {
        [Test]
        public void Test()
        {
            var bus = new Bus<Int32>();
            bus.SendLocal(34);

/*            bus.Where(i => i > 100).Subscribe(i =>
            {
                Console.WriteLine("New Message Arrived {0}", i);
            });

            var dis = bus.Subscribe(i =>
            {
                Console.WriteLine("||| New Message Arrived {0}", i);
            });*/

            bus.SendLocal(346);
            bus.Run();

            bus.SendLocal(777);

//            dis.Dispose();

            bus.SendLocal(888);



            
        }
    }
}
