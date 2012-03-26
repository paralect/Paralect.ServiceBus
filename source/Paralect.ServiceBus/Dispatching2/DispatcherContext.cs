using Microsoft.Practices.ServiceLocation;

namespace Paralect.ServiceBus.Dispatching2
{
    public class DispatcherContext
    {
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Breaks invocations
        /// </summary>
        public void Break()
        {
            
        }

        /// <summary>
        /// This is default behavier
        /// </summary>
        public void Continue()
        {
            
        }

        public void Dispatch()
        {
            
        }
    }
}