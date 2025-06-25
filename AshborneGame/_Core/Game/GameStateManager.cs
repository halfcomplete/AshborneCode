
namespace AshborneGame._Core.Game
{
    /// <summary>
    /// Centralised state manager for tracking global flags, counters, and custom variables.
    /// Ideal for quest logic, event triggers, and persistent world state.
    /// </summary>
    public class GameStateManager
    {
        public Dictionary<string, bool> Flags { get; private set; } = new();
        public Dictionary<string, int> Counters { get; private set; } = new();
        public Dictionary<string, object> Variables { get; private set; } = new();

        // ----------- FLAGS (True/False binary states) -----------

        public void SetFlag(string key, bool value) => Flags[key] = value;

        /// <summary>
        /// Gets the value of the provided flag name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A bool? value, true if the flag is true, false if the flag is false, null if the flag doesn't exist.</returns>
        public bool? GetFlag(string key) { return Flags.TryGetValue(key, out var value) ? value : null; }

        /// <summary>
        /// Gets whether the flag exists.
        /// </summary>
        public bool HasFlag(string key) => Flags.ContainsKey(key);

        /// <summary>
        /// Removes the flag.
        /// </summary>
        public void RemoveFlag(string key) => Flags.Remove(key);

        /// <summary>
        /// Toggles a flag. If it was true, this sets it to false, else it's set to true.
        /// </summary>
        /// <returns>True if the flag is now true. False if the flag is now false. Null if the flag doesn't exist.</returns>
        public bool? TryToggleFlag(string key)
        {
            if (Flags.ContainsKey(key))
            {
                Flags[key] = !Flags[key];
                return true;
            }
            else
                return null;
        }

        // ----------- COUNTERS (Integers with mutations) -----------

        public void SetCounter(string key, int value) => Counters[key] = value;

        /// <summary>
        /// Gets the value of a counter.
        /// </summary>
        /// <returns>The value if it exists. Null otherwise.</returns>
        public int? TryGetCounter(string key) => Counters.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// Increments a counter by amount.
        /// </summary>
        /// <returns>True if it was successful. False otherwise.</returns>
        public bool TryIncrementCounter(string key, int amount = 1)
        {
            int? baseValue = TryGetCounter(key);
            if (baseValue == null)
            {
                return false;
            }
            Counters[key] = (int)baseValue + amount;
            return true;
        }

        /// <summary>
        /// Decrements a counter by an amount. Sets it to 0 if it goes below 0.
        /// </summary>
        /// <returns>True if it was successful. False otherwise.</returns>
        public bool TryDecrementCounter(string key, int amount = 1)
        {
            int? baseValue = TryGetCounter(key);
            if (baseValue == null)
            {
                return false;
            }
            Counters[key] = (int)baseValue - amount;
            if (Counters[key] < 0) Counters[key] = 0;
            return true;
        }

        public bool HasCounter(string key) => Counters.ContainsKey(key);

        public void RemoveCounter(string key) => Counters.Remove(key);

        // ----------- VARIABLES (Generic object storage) -----------

        public void SetVariable(string key, object value) => Variables[key] = value;

        /// <summary>
        /// Attempts to get the value of the variable.
        /// </summary>
        /// <returns>The value if successful, null otherwise.</returns>
        public object? TryGetVariable(string key) =>
            Variables.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// Attempts to get the value of variable provided it is the target 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetVariable<T>(string key)
        {
            if (Variables.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default;
        }

        public bool HasVariable(string key) => Variables.ContainsKey(key);

        public void RemoveVariable(string key) => Variables.Remove(key);

        // ----------- UTILITIES -----------

        public void ClearAll()
        {
            Flags.Clear();
            Counters.Clear();
            Variables.Clear();
        }

        public override string ToString()
        {
            return $"[Flags: {Flags.Count}, Counters: {Counters.Count}, Variables: {Variables.Count}]";
        }
    }
}
