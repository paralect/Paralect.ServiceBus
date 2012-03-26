using System;

namespace Paralect.ServiceBus.Dispatching2
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        void Dispatch(Object message);
    }
}