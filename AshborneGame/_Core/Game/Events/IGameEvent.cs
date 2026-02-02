namespace AshborneGame._Core.Game.Events
{
    /// <summary>
    /// Marker interface for all game events. All events must implement this interface
    /// to be publishable and subscribable through the EventBus.
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// If true, all subscribers are removed after the event is published once.
        /// The event can still be published again, but no handlers will receive it
        /// unless new subscriptions are made.
        /// </summary>
        bool OneTime => false;
    }

    /// <summary>
    /// Strongly-typed marker interface for events with a specific payload type.
    /// Use this when you want compile-time enforcement of event data types.
    /// </summary>
    /// <typeparam name="TSelf">The event type itself (CRTP pattern for type safety).</typeparam>
    public interface IGameEvent<TSelf> : IGameEvent where TSelf : IGameEvent<TSelf>
    {
    }
}
