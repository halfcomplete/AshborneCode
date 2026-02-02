namespace AshborneGame._Core.Game.Events
{
    /// <summary>
    /// Factory and container for all game event types.
    /// Events are organized by domain/context for discoverability.
    /// </summary>
    /// <remarks>
    /// <para><b>Creating a new event:</b></para>
    /// <code>
    /// // 1. Add a sealed record implementing IGameEvent in the appropriate nested class
    /// public sealed record MyNewEvent(string Data) : IGameEvent;
    /// 
    /// // 2. For one-time events, override the OneTime property
    /// public sealed record MyOneTimeEvent() : IGameEvent { public bool OneTime => true; }
    /// </code>
    /// 
    /// <para><b>Publishing:</b></para>
    /// <code>
    /// new GameEvents.Player.MovedEvent(location).Publish();
    /// // or
    /// EventBus.Publish(new GameEvents.Player.MovedEvent(location));
    /// </code>
    /// </remarks>
    public static class GameEvents
    {
        /// <summary>
        /// Events related to Ossaneth's Domain area.
        /// </summary>
        public static class OssanethsDomain
        {
            /// <summary>
            /// Raised when the outro sequence of Ossaneth's Domain is triggered.
            /// This is a one-time event - all subscribers are removed after first publish.
            /// </summary>
            public sealed record OutroTriggeredEvent() : IGameEvent
            {
                public bool OneTime => true;
            }

            /// <summary>
            /// Raised when the Eye Platform visit count reaches a specific threshold (e.g., 4th visit).
            /// </summary>
            /// <param name="LocationName">The name of the location.</param>
            /// <param name="VisitCount">The current visit count.</param>
            public sealed record EyePlatformVisitThresholdEvent(string LocationName, int VisitCount) : IGameEvent
            {
                public bool OneTime => true;
            }
        }

        /// <summary>
        /// Events related to player actions.
        /// </summary>
        public static class Player
        {
            /// <summary>
            /// Raised when the player prays at a location.
            /// </summary>
            /// <param name="LocationGroup">The location group where the player prayed.</param>
            public sealed record PrayedEvent(string LocationGroup) : IGameEvent;

            /// <summary>
            /// Raised when the player moves to a new location.
            /// </summary>
            /// <param name="FromLocation">The location the player moved from (null if initial spawn).</param>
            /// <param name="ToLocation">The location the player moved to.</param>
            public sealed record MovedEvent(string? FromLocation, string ToLocation) : IGameEvent;

            /// <summary>
            /// Raised when the player picks up an item.
            /// </summary>
            /// <param name="ItemName">The name of the item picked up.</param>
            /// <param name="ItemId">The unique ID of the item.</param>
            public sealed record ItemPickedUpEvent(string ItemName, string ItemId) : IGameEvent;

            /// <summary>
            /// Raised when the player equips a mask.
            /// </summary>
            /// <param name="MaskName">The name of the mask equipped.</param>
            public sealed record MaskEquippedEvent(string MaskName) : IGameEvent;
        }

        /// <summary>
        /// Events related to quests.
        /// </summary>
        public static class Quest
        {
            /// <summary>
            /// Raised when a quest is started.
            /// </summary>
            /// <param name="QuestId">The unique ID of the quest.</param>
            /// <param name="QuestName">The display name of the quest.</param>
            public sealed record StartedEvent(string QuestId, string QuestName) : IGameEvent;

            /// <summary>
            /// Raised when a quest is completed successfully.
            /// </summary>
            /// <param name="QuestId">The unique ID of the quest.</param>
            /// <param name="QuestName">The display name of the quest.</param>
            public sealed record CompletedEvent(string QuestId, string QuestName) : IGameEvent;

            /// <summary>
            /// Raised when a quest fails.
            /// </summary>
            /// <param name="QuestId">The unique ID of the quest.</param>
            /// <param name="QuestName">The display name of the quest.</param>
            /// <param name="Reason">The reason for failure.</param>
            public sealed record FailedEvent(string QuestId, string QuestName, string? Reason = null) : IGameEvent;

            /// <summary>
            /// Raised when a quest objective is updated.
            /// </summary>
            /// <param name="QuestId">The quest ID.</param>
            /// <param name="ObjectiveId">The objective ID that was updated.</param>
            /// <param name="Progress">Current progress value.</param>
            /// <param name="Target">Target value for completion.</param>
            public sealed record ObjectiveUpdatedEvent(string QuestId, string ObjectiveId, int Progress, int Target) : IGameEvent;
        }

        /// <summary>
        /// Events related to dialogue and conversations.
        /// </summary>
        public static class Dialogue
        {
            /// <summary>
            /// Raised when a dialogue sequence starts.
            /// </summary>
            /// <param name="DialogueName">The name/identifier of the dialogue.</param>
            public sealed record StartedEvent(string DialogueName) : IGameEvent;

            /// <summary>
            /// Raised when a dialogue sequence ends.
            /// </summary>
            /// <param name="DialogueName">The name/identifier of the dialogue.</param>
            public sealed record EndedEvent(string DialogueName) : IGameEvent;

            /// <summary>
            /// Raised when the player makes a choice in dialogue.
            /// </summary>
            /// <param name="DialogueName">The dialogue containing the choice.</param>
            /// <param name="ChoiceIndex">The index of the choice made.</param>
            /// <param name="ChoiceText">The text of the choice made.</param>
            public sealed record ChoiceMadeEvent(string DialogueName, int ChoiceIndex, string ChoiceText) : IGameEvent;
        }

        /// <summary>
        /// Events related to game state and system.
        /// </summary>
        public static class System
        {
            /// <summary>
            /// Raised when the game is saved.
            /// </summary>
            /// <param name="SlotName">The save slot name.</param>
            public sealed record GameSavedEvent(string SlotName) : IGameEvent;

            /// <summary>
            /// Raised when the game is loaded.
            /// </summary>
            /// <param name="SlotName">The save slot name.</param>
            public sealed record GameLoadedEvent(string SlotName) : IGameEvent;

            /// <summary>
            /// Raised when a game tick occurs.
            /// </summary>
            /// <param name="DeltaTime">Time elapsed since last tick.</param>
            public sealed record TickEvent(TimeSpan DeltaTime) : IGameEvent;
        }
    }

    /// <summary>
    /// Legacy factory class - use <see cref="GameEvents"/> instead.
    /// </summary>
    [Obsolete("Use GameEvents nested classes directly. E.g., new GameEvents.OssanethsDomain.OutroTriggeredEvent()")]
    public static class GameEventFactory
    {
        public static class OssanethsDomain
        {
            [Obsolete("Use new GameEvents.OssanethsDomain.OutroTriggeredEvent() instead.")]
            public static GameEvents.OssanethsDomain.OutroTriggeredEvent CreateOssanethsDomainOutroTriggeredEvent() 
                => new GameEvents.OssanethsDomain.OutroTriggeredEvent();
        }
    }
}