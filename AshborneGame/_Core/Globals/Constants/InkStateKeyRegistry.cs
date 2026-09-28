using AshborneGame._Core.Game;

namespace AshborneGame._Core.Globals.Constants
{
    /// <summary>
    /// Compatibility facade for Ink and tooling. Key registration and validation are owned by GameStateTracker.
    /// </summary>
    public static class InkStateKeyRegistry
    {
        public static GameStateKey<bool> ValidateAndGetFlagKey(string inkStringKey) =>
            GameStateTracker.ValidateAndGetFlagKey(inkStringKey);

        public static GameStateKey<int> ValidateAndGetCounterKey(string inkStringKey) =>
            GameStateTracker.ValidateAndGetCounterKey(inkStringKey);

        public static GameStateKey<string> ValidateAndGetLabelKey(string inkStringKey) =>
            GameStateTracker.ValidateAndGetLabelKey(inkStringKey);

        public static IEnumerable<string> GetAllRegisteredFlagKeys() =>
            GameStateTracker.GetAllRegisteredFlagKeys();

        public static IEnumerable<string> GetAllRegisteredCounterKeys() =>
            GameStateTracker.GetAllRegisteredCounterKeys();

        public static IEnumerable<string> GetAllRegisteredLabelKeys() =>
            GameStateTracker.GetAllRegisteredLabelKeys();
    }
}
