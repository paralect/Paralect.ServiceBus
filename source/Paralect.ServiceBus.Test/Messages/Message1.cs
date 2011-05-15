using System;

namespace Paralect.ServiceBus.Test.Messages
{
    public class Message1
    {
        public String Name { get; set; }
        public Int32 Year { get; set; }

        /// <summary>
        /// Default constractor for serialization
        /// </summary>
        public Message1() { }
        public Message1(string name, int year)
        {
            Name = name;
            Year = year;
        }
    }
}