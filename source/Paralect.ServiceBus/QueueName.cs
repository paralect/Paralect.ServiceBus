using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paralect.ServiceBus
{
    public class QueueName
    {
        public String Name { get; set; }
        public String ComputerName { get; set; }

        /// <summary>
        /// Should be in the format:
        /// QueueName@ComputerName
        /// </summary>
        public QueueName(String path)
        {
            var parts = path.Split('@');
            Name = parts[0];
            ComputerName = (parts.Length == 2) ? parts[1] : ".";
        }

        public String GetQueuePath()
        {
            return String.Format(@"{0}\private$\{1}", ComputerName, Name);
        }
    }
}
