namespace Paralect.ServiceBus.Dispatching2
{
    /// <summary>
    /// Dispatching mode
    /// </summary>
    public enum DispatchMode
    {
        /// <summary>
        /// Types need to match exactly in order to be dispatched to handler
        /// </summary>
        ExactType = 1,

        /// <summary>
        /// If you subscribed on class type - only messages with exactly this class types will be dispatched to handler. Hierarchy for class types is not important.
        /// If you subscribed on interface type - you automaticaly subscribed to all descedant types (both classes and interfaces)
        /// Default mode.
        /// </summary>
        InterfaceDescendants = 2,

        /// <summary>
        /// Regardless of subscription on class or interface, you automaticaly subscribed to all descedant types (both classes and interfaces)
        /// </summary>
        TypeDescendant = 3
    }
}