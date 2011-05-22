using System;

namespace Paralect.ServiceBus
{
    public class TransportMessage
    {
        public String SentFromComputerName { get; set; }
        public String SentFromQueueName { get; set; }
        public Object[] Messages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransportMessage(params Object[] messages)
        {
            Messages = messages;

    /*
	        using (IServiceBus bus = ServiceBusFactory.New(x => x
                       .ReceiveFrom("loopback://localhost/my_queue")
			           .SetReceiveTimeout(10.Milliseconds())
			           .Environments(e => e
                            .Add<Production>()
                            .Select("production")
				       );
		        ))
	        {
		        ((ServiceBus) bus).MaximumConsumerThreads.ShouldEqual(7);
	        }*/

        }
    }
}





