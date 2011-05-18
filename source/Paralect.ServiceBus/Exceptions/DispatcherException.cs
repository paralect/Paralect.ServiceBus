using System;

namespace Paralect.ServiceBus.Exceptions
{
    public class DispatcherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public DispatcherException(string message) : base(message)
        {

        }
    }
}
