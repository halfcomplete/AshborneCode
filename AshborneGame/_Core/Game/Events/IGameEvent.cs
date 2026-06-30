using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.Game.Events
{
    public sealed record MemoryParticipant(Guid EntityId, List<MemoryRole> Roles);

    /// <summary>
    /// Marker interface for all game events. All events must implement this interface
    /// to be publishable and subscribable through the EventBus.
    /// </summary>
    public interface IGameEvent
    {
        public int CurrentTotalHours { get; }
        /// <summary>
        /// If true, all subscribers are removed after the event is published once.
        /// The event can still be published again, but no handlers will receive it
        /// unless new subscriptions are made.
        /// </summary>
        bool OneTime => false;
    }

    /// <summary>
    /// Marker interface for all game events that are able to create Memories in NPCs.
    /// </summary>
    /// <remarks>
    /// Note that when defining new IMemorableGameEvent record types, the MemoryDefinition must be specified as a default value.
    /// On the other hand, Participants and other data specific to that particular GameEvent are to be passed in.
    /// </remarks>
    public interface IMemorableGameEvent : IGameEvent
    {
        public MemoryDefinition MemoryDefinition { get; }
        public List<MemoryParticipant> Participants { get; }
        
    }

    /// <summary>
    /// Strongly-typed marker interface for events with a specific payload type.
    /// Use this when compile-time enforcement of event data types is wanted.
    /// </summary>
    /// <typeparam name="TSelf">The event type itself (CRTP pattern for type safety).</typeparam>
    public interface IGameEvent<TSelf> : IGameEvent where TSelf : IGameEvent<TSelf>
    {
    }
}
