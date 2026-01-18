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
        public static class Ossaneth
        {
            public static class Domain
            {
                /// <summary>
                /// Raised when the player has visited 4 unique locations in Ossaneth's Domain.
                /// </summary>
                public const string OnEyePlatformVisitCountEqualFour = "Events.Ossaneth.Domain.OnEyePlatformVisitCountEqualFour";

                /// <summary>
                /// Raised when the outro sequence should trigger (visited 2+ dreamspace locations).
                /// Event data:
                ///   - "visited_count" (int): Number of dreamspace locations visited
                ///   - "location_name" (string): Reference name of the current location
                /// </summary>
                public const string OnOutroTriggered = "Events.Ossaneth.Domain.OnOutroTriggered";
            }
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
