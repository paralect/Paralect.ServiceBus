using System;

namespace Paralect.ServiceBus.Test.Messages
{
    public interface ISuperBaseMessage
    {
        String Name { get; set; }
    }

    public interface IBaseMessage : ISuperBaseMessage
    {

    }

    public class Message1 : IBaseMessage
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