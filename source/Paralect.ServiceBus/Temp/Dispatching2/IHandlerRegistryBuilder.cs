namespace Paralect.ServiceBus.Dispatching2
{
    public interface IHandlerRegistryBuilder
    {
        /// <summary>
        /// Register handler
        /// </summary>
        void Register(IHandler handler);

        /// <summary>
        /// Unregister handler
        /// </summary>
        void Unregister(IHandler handler);

        /// <summary>
        /// Build handler registry
        /// </summary>
        IHandlerRegistry BuildHandlerRegistry();
    }
}