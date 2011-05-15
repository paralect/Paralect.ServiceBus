using System;

namespace Paralect.ServiceBus.Test.Messages
{
    public class Message2
    {
        public String Title { get; set; }
        public Int32 Height { get; set; }
        public Message1 InnerMessage { get; set; }

        /// <summary>
        /// Default constractor for serialization
        /// </summary>
        public Message2() { }
        public Message2(string title, int height)
        {
            Title = title;
            Height = height;
            InnerMessage = new Message1(title + title, height + height);
        }
    }
}