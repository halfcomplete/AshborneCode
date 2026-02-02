namespace AshborneGame._Core.Globals.Constants
{
    /// <summary>
    /// Centralized, type-safe event name constants for the game event system.
    /// All event names used with EventBus should be referenced from this class to ensure consistency
    /// and enable compile-time safety checks across the codebase.
    /// </summary>
    public static class EventNameConstants
    {
        /// <summary>
        /// Events related to Ossaneth's Domain and dream sequence progression.
        /// </summary>
        public static class OssanethsDomain
        {
            /// <summary>
            /// Raised when the outro sequence should trigger.<br/>
            /// </summary>
            /// <remarks>
            /// No event data.
            /// </remarks>
            public const string OnOutroTriggered = "Events.OssanethsDomain.OnOutroTriggered";
        }

        /// <summary>
        /// Events related to player actions and state changes.
        /// </summary>
        public static class Player
        {
            /// <summary>
            /// Events triggered by the player's interactions with the game world.
            /// </summary>
            public static class Actions
            {
                /// <summary>
                /// Raised when the player prays (location-specific action).
                /// Event data: 
                ///   - "location_group" (string): The location group where the prayer occurred (e.g., "Ossaneth's Domain")
                /// </summary>
                public const string Prayed = "Events.Player.Actions.OnPrayed";
            }   
        }
    }
}
