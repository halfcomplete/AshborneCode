using System;
using System.Reflection;
using System.Collections.Generic;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Constants
{
    /// <summary>
    /// Registry that bridges the gap between Ink's string-based state access and C#'s type-safe GameStateKey system.
    /// 
    /// Ink scripts can only pass strings when calling external C# functions. This registry validates those string keys
    /// against known identifiers defined in StateKeys, throwing exceptions for unknown keys.
    /// 
    /// This ensures that:
    /// 1. Typos in Ink scripts are caught at runtime (fail-fast)
    /// 2. All possible Ink state keys are centrally documented
    /// 3. There's a single mapping point for Ink string → C# GameStateKey conversions
    /// </summary>
    public static class InkStateKeyRegistry
    {
        // Precomputed dictionaries for O(1) lookups during dialogue execution
        private static readonly Dictionary<string, GameStateKey<bool>> FlagRegistry = new();
        private static readonly Dictionary<string, GameStateKey<int>> CounterRegistry = new();
        private static readonly Dictionary<string, GameStateKey<string>> LabelRegistry = new();

        static InkStateKeyRegistry()
        {
            RegisterFlags();
            RegisterCounters();
            RegisterLabels();
        }

        public static List<FieldInfo> GetAllStaticFields(Type type)
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            // 1. Get static fields in the current type
            // Use BindingFlags.Static, BindingFlags.Public, BindingFlags.NonPublic to get all access levels
            fields.AddRange(type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));

            // 2. Get all nested types (classes/structs) in the current type
            Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

            foreach (Type nestedType in nestedTypes)
            {
                // A static class in C# is internally represented as a sealed and abstract class
                // This condition checks if the nested type is a static class
                if (nestedType.IsAbstract && nestedType.IsSealed)
                {
                    // Recursively call the method for nested static classes
                    fields.AddRange(GetAllStaticFields(nestedType));
                }
            }

            return fields;
        }

        private static void RegisterFlags()
        {
            Type flagsStaticClass = typeof(StateKeys.Flags);
            // Recursively get all the static fields in the Flags static class and its nested static classes
            var staticFields = GetAllStaticFields(flagsStaticClass);

            foreach (var field in staticFields)
            {
                if (field.FieldType == typeof(GameStateKey<bool>))
                {
                    object? value = field.GetValue(null);
                    if (value == null)
                    {
                        IOService.Output.DisplayDebugMessage($"Field '{field.Name}' in '{flagsStaticClass.FullName}' is null. Skipping registration.", ConsoleMessageTypes.WARNING);
                        continue;
                    }
                    var flagKey = (GameStateKey<bool>)value;
                    RegisterFlag(flagKey);
                }
            }

            RegisterFlag(new GameStateKey<bool>("Flags.TestFlag"));
        }

        private static void RegisterCounters()
        {
            Type countersStaticClass = typeof(StateKeys.Counters);
            // Recursively get all the static fields in the Counters static class and its nested static classes
            var staticFields = GetAllStaticFields(countersStaticClass);

            foreach (var field in staticFields)
            {
                if (field.FieldType == typeof(GameStateKey<int>))
                {
                    object? value = field.GetValue(null);
                    if (value == null)
                    {
                        IOService.Output.DisplayDebugMessage($"Field '{field.Name}' in '{countersStaticClass.FullName}' is null. Skipping registration.", ConsoleMessageTypes.WARNING);
                        continue;
                    }
                    var counterKey = (GameStateKey<int>)value;
                    RegisterCounter(counterKey);
                }
            }

            RegisterCounter(new GameStateKey<int>("Counters.TestCounter"));
        }

        private static void RegisterLabels()
        {
            Type labelsStaticClass = typeof(StateKeys.Labels);
            // Recursively get all the static fields in the Labels static class and its nested static classes
            var staticFields = GetAllStaticFields(labelsStaticClass);

            foreach (var field in staticFields)
            {
                if (field.FieldType == typeof(GameStateKey<string>))
                {
                    object? value = field.GetValue(null);
                    if (value == null)
                    {
                        IOService.Output.DisplayDebugMessage($"Field '{field.Name}' in '{labelsStaticClass.FullName}' is null. Skipping registration.", ConsoleMessageTypes.WARNING);
                        continue;
                    }
                    var labelKey = (GameStateKey<string>)value;
                    RegisterLabel(labelKey);
                }
            }

            RegisterLabel(new GameStateKey<string>("Labels.TestLabel"));
        }

        private static void RegisterFlag(GameStateKey<bool> flagKey)
        {
            FlagRegistry.Add(flagKey.Key, flagKey);
        }

        private static void RegisterCounter(GameStateKey<int> counterKey)
        {
            CounterRegistry.Add(counterKey.Key, counterKey);
        }

        private static void RegisterLabel(GameStateKey<string> labelKey)
        {
            LabelRegistry.Add(labelKey.Key, labelKey);
        }


        /// <summary>
        /// Validates and retrieves a flag key from the registry.
        /// Throws an exception if the key is unknown to prevent silent state corruption.
        /// </summary>
        public static GameStateKey<bool> ValidateAndGetFlagKey(string inkStringKey)
        {
            if (string.IsNullOrWhiteSpace(inkStringKey))
                throw new ArgumentException("Ink flag key cannot be null or empty", nameof(inkStringKey));

            // First check exact matches in registry
            if (FlagRegistry.TryGetValue(inkStringKey, out var key))
                return key;

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown flag key from Ink: '{inkStringKey}'. " +
                $"All flag keys must be registered as a readonly static field in {nameof(StateKeys.Flags)}. " +
                $"This is likely a typo in the Ink script.");
        }

        /// <summary>
        /// Validates and retrieves a counter key from the registry.
        /// Throws an exception if the key is unknown to prevent silent state corruption.
        /// </summary>
        public static GameStateKey<int> ValidateAndGetCounterKey(string inkStringKey)
        {
            if (string.IsNullOrWhiteSpace(inkStringKey))
                throw new ArgumentException("Ink counter key cannot be null or empty", nameof(inkStringKey));

            // First check exact matches in registry
            if (CounterRegistry.TryGetValue(inkStringKey, out var key))
                return key;

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown counter key from Ink: '{inkStringKey}'. " +
                $"All counter keys must be registered as a readonly static field in {nameof(StateKeys.Counters)}. " +
                $"This is likely a typo in the Ink script. Known counters: {string.Join(", ", CounterRegistry.Keys)}");
        }

        /// <summary>
        /// Validates and retrieves a label key from the registry.
        /// Throws an exception if the key is unknown to prevent silent state corruption.
        /// </summary>
        public static GameStateKey<string> ValidateAndGetLabelKey(string inkStringKey)
        {
            if (string.IsNullOrWhiteSpace(inkStringKey))
                throw new ArgumentException("Ink label key cannot be null or empty", nameof(inkStringKey));

            // First check exact matches in registry
            if (LabelRegistry.TryGetValue(inkStringKey, out var key))
                return key;

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown label key from Ink: '{inkStringKey}'. " +
                $"All label keys must be registered as a readonly static field in {nameof(StateKeys.Labels)}. " +
                $"This is likely a typo in the Ink script.");
        }

        /// <summary>
        /// Logs a warning message using the IOService if available.
        /// </summary>
        private static void LogWarning(string message)
        {
            try
            {
                if (IOService.Output != null)
                {
                    IOService.Output.DisplayDebugMessage(message, ConsoleMessageTypes.WARNING).GetAwaiter().GetResult();
                }
            }
            catch
            {
                // If logging fails, silently continue - don't let registry fail due to logging issues
                System.Diagnostics.Debug.WriteLine($"[InkStateKeyRegistry Warning] {message}");
            }
        }

        /// <summary>
        /// Returns all registered flag keys (for debugging and validation tools).
        /// </summary>
        public static IEnumerable<string> GetAllRegisteredFlagKeys() => FlagRegistry.Keys;

        /// <summary>
        /// Returns all registered counter keys (for debugging and validation tools).
        /// </summary>
        public static IEnumerable<string> GetAllRegisteredCounterKeys() => CounterRegistry.Keys;

        /// <summary>
        /// Returns all registered label keys (for debugging and validation tools).
        /// </summary>
        public static IEnumerable<string> GetAllRegisteredLabelKeys() => LabelRegistry.Keys;
    }
}
