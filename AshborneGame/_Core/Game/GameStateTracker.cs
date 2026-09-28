using System.Reflection;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.Game
{
    /// <summary>
    /// Single source of truth for registered and live flags, counters, and labels.
    /// </summary>
    public sealed class GameStateTracker
    {
        private static readonly Dictionary<string, GameStateKey<bool>> FlagKeys = new();
        private static readonly Dictionary<string, GameStateKey<int>> CounterKeys = new();
        private static readonly Dictionary<string, GameStateKey<string>> LabelKeys = new();

        static GameStateTracker()
        {
            RegisterKeys(typeof(StateKeys.Flags), FlagKeys);
            RegisterKeys(typeof(StateKeys.Counters), CounterKeys);
            RegisterKeys(typeof(StateKeys.Labels), LabelKeys);

            FlagKeys.TryAdd("Flags.TestFlag", new GameStateKey<bool>("Flags.TestFlag"));
            CounterKeys.TryAdd("Counters.TestCounter", new GameStateKey<int>("Counters.TestCounter"));
            LabelKeys.TryAdd("Labels.TestLabel", new GameStateKey<string>("Labels.TestLabel"));
        }

        public Dictionary<string, bool> Flags { get; } = new();
        public Dictionary<string, int> Counters { get; } = new();
        public Dictionary<string, string> Labels { get; } = new();

        public static GameStateKey<bool> ValidateAndGetFlagKey(string key) => Validate(key, FlagKeys, "flag");
        public static GameStateKey<int> ValidateAndGetCounterKey(string key) => Validate(key, CounterKeys, "counter");
        public static GameStateKey<string> ValidateAndGetLabelKey(string key) => Validate(key, LabelKeys, "label");

        public static IEnumerable<string> GetAllRegisteredFlagKeys() => FlagKeys.Keys;
        public static IEnumerable<string> GetAllRegisteredCounterKeys() => CounterKeys.Keys;
        public static IEnumerable<string> GetAllRegisteredLabelKeys() => LabelKeys.Keys;

        public void SetFlag(GameStateKey<bool> key, bool value) => Flags[key] = value;
        public bool TryGetFlag(GameStateKey<bool> key, out bool value) => Flags.TryGetValue(key, out value);
        public bool HasFlag(GameStateKey<bool> key) => Flags.ContainsKey(key);
        public bool RemoveFlag(GameStateKey<bool> key) => Flags.Remove(key);

        public bool? TryToggleFlag(GameStateKey<bool> key)
        {
            if (!Flags.TryGetValue(key, out var value))
                return null;

            Flags[key] = !value;
            return Flags[key];
        }

        public void SetCounter(GameStateKey<int> key, int value) => Counters[key] = value;
        public bool TryGetCounter(GameStateKey<int> key, out int value) => Counters.TryGetValue(key, out value);
        public bool HasCounter(GameStateKey<int> key) => Counters.ContainsKey(key);
        public bool RemoveCounter(GameStateKey<int> key) => Counters.Remove(key);

        public bool TryIncrementCounter(GameStateKey<int> key, int amount = 1)
        {
            if (!Counters.TryGetValue(key, out var value))
                return false;

            Counters[key] = value + amount;
            return true;
        }

        public bool TryDecrementCounter(GameStateKey<int> key, int amount = 1)
        {
            if (!Counters.TryGetValue(key, out var value))
                return false;

            Counters[key] = Math.Max(0, value - amount);
            return true;
        }

        public void SetLabel(GameStateKey<string> key, string value) => Labels[key] = value;
        public string? TryGetLabel(GameStateKey<string> key) => Labels.TryGetValue(key, out var value) ? value : null;
        public bool HasLabel(GameStateKey<string> key) => Labels.ContainsKey(key);
        public bool RemoveLabel(GameStateKey<string> key) => Labels.Remove(key);

        public void Clear()
        {
            Flags.Clear();
            Counters.Clear();
            Labels.Clear();
        }

        private static void RegisterKeys<T>(Type type, Dictionary<string, GameStateKey<T>> registry)
        {
            foreach (var field in GetAllStaticFields(type))
            {
                if (field.FieldType != typeof(GameStateKey<T>) || field.GetValue(null) is not GameStateKey<T> key)
                    continue;

                registry.TryAdd(key.Key, key);
            }
        }

        private static List<FieldInfo> GetAllStaticFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).ToList();
            foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (nestedType.IsAbstract && nestedType.IsSealed)
                    fields.AddRange(GetAllStaticFields(nestedType));
            }

            return fields;
        }

        private static TKey Validate<TKey>(string key, Dictionary<string, TKey> registry, string kind)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"Ink {kind} key cannot be null or empty", nameof(key));

            if (registry.TryGetValue(key, out var registeredKey))
                return registeredKey;

            throw new InvalidOperationException($"Unknown {kind} key from Ink: '{key}'. Known {kind} keys: {string.Join(", ", registry.Keys)}");
        }
    }
}