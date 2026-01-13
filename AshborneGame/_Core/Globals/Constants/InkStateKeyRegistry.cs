using System;
using System.Collections.Generic;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Constants
{
    /// <summary>
    /// Registry that bridges the gap between Ink's string-based state access and C#'s type-safe GameStateKey system.
    /// 
    /// Ink scripts can only pass strings when calling external C# functions. This registry validates those string keys
    /// against known identifiers defined in GameStateKeyConstants, throwing exceptions for unknown keys.
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
            // Register all valid flag keys from GameStateKeyConstants
            RegisterFlags();
            
            // Register all valid counter keys from GameStateKeyConstants
            RegisterCounters();
            
            // Register all valid label keys from GameStateKeyConstants
            RegisterLabels();
        }

        private static void RegisterFlags()
        {
            // Flags: Player actions in Ossaneth's Domain
            FlagRegistry.Add(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors, 
                GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors);
            FlagRegistry.Add(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound, 
                GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound);
            FlagRegistry.Add(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom, 
                GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom);
            FlagRegistry.Add(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks, 
                GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks);
            FlagRegistry.Add(GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_TalkedToBoundOne, 
                GameStateKeyConstants.Flags.Player.Actions.In.OssanethDreamspace_TalkedToBoundOne);

            // Additional flags that appear in Ink scripts (dynamically set strings)
            // These are common patterns that Ink scripts will use. Register them as "known unknown" patterns
            // that we allow through but log warnings for.
            RegisterDynamicFlagPatterns();
        }

        private static void RegisterCounters()
        {
            // Counters: Player progression
            CounterRegistry.Add(GameStateKeyConstants.Counters.Player.CurrentSceneNo, 
                GameStateKeyConstants.Counters.Player.CurrentSceneNo);
            CounterRegistry.Add(GameStateKeyConstants.Counters.Player.CurrentActNo, 
                GameStateKeyConstants.Counters.Player.CurrentActNo);

            // Additional dynamic counters from Ink
            RegisterDynamicCounterPatterns();
        }

        private static void RegisterLabels()
        {
            // Labels: Player state strings
            LabelRegistry.Add(GameStateKeyConstants.Labels.Player.Name, 
                GameStateKeyConstants.Labels.Player.Name);
            LabelRegistry.Add(GameStateKeyConstants.Labels.Player.Input, 
                GameStateKeyConstants.Labels.Player.Input);
        }

        /// <summary>
        /// Registers dynamic flag patterns that Ink scripts commonly use.
        /// These are patterns like "player.received_ossaneth", "player.actions.*", etc.
        /// While not explicitly defined in constants, we whitelist these common prefixes.
        /// </summary>
        private static void RegisterDynamicFlagPatterns()
        {
            // Allow any flag starting with these prefixes as they're set dynamically by Ink
            // Common pattern: "player.actions.talked.to_*", "player.actions.*", etc.
            // These will be validated with a prefix check instead of exact match
        }

        /// <summary>
        /// Registers dynamic counter patterns that Ink scripts commonly use.
        /// These include counters like "player.prayers", "player.masks_count", etc.
        /// </summary>
        private static void RegisterDynamicCounterPatterns()
        {
            // Common counters that are set by Ink but not defined as constants
            // These follow a standard naming pattern: "player.*" or similar
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

            // Check if it matches known dynamic patterns
            if (IsValidDynamicFlagKey(inkStringKey))
            {
                LogWarning($"Flag key '{inkStringKey}' is not in the known constants registry but matches a whitelisted pattern. " +
                    "Consider adding it to GameStateKeyConstants for better type safety.");
                return new GameStateKey<bool>(inkStringKey);
            }

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown flag key from Ink: '{inkStringKey}'. " +
                $"All flag keys must be registered in {nameof(InkStateKeyRegistry)} or {nameof(GameStateKeyConstants)}. " +
                $"This is likely a typo in an Ink script. Known flags: {string.Join(", ", FlagRegistry.Keys)}");
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

            // Check if it matches known dynamic patterns
            if (IsValidDynamicCounterKey(inkStringKey))
            {
                LogWarning($"Counter key '{inkStringKey}' is not in the known constants registry but matches a whitelisted pattern. " +
                    "Consider adding it to GameStateKeyConstants for better type safety.");
                return new GameStateKey<int>(inkStringKey);
            }

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown counter key from Ink: '{inkStringKey}'. " +
                $"All counter keys must be registered in {nameof(InkStateKeyRegistry)} or {nameof(GameStateKeyConstants)}. " +
                $"This is likely a typo in an Ink script. Known counters: {string.Join(", ", CounterRegistry.Keys)}");
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

            // Check if it matches known dynamic patterns
            if (IsValidDynamicLabelKey(inkStringKey))
            {
                LogWarning($"Label key '{inkStringKey}' is not in the known constants registry but matches a whitelisted pattern. " +
                    "Consider adding it to GameStateKeyConstants for better type safety.");
                return new GameStateKey<string>(inkStringKey);
            }

            // Unknown key - fail fast
            throw new InvalidOperationException(
                $"Unknown label key from Ink: '{inkStringKey}'. " +
                $"All label keys must be registered in {nameof(InkStateKeyRegistry)} or {nameof(GameStateKeyConstants)}. " +
                $"This is likely a typo in an Ink script. Known labels: {string.Join(", ", LabelRegistry.Keys)}");
        }

        /// <summary>
        /// Checks if a flag key matches known dynamic patterns.
        /// Allows common Ink patterns that aren't explicitly registered.
        /// </summary>
        private static bool IsValidDynamicFlagKey(string key)
        {
            // Pattern: player.actions.* (any player action tracking)
            if (key.StartsWith("player.actions."))
                return true;

            // Pattern: player.received_* (tracking receipt of items/masks)
            if (key.StartsWith("player.received_"))
                return true;

            // Pattern: Flags.Player.* (dynamically created flags)
            if (key.StartsWith("Flags.Player."))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a counter key matches known dynamic patterns.
        /// Allows common Ink patterns that aren't explicitly registered.
        /// </summary>
        private static bool IsValidDynamicCounterKey(string key)
        {
            // Pattern: player.* (any player counter)
            if (key.StartsWith("player."))
                return true;

            // Pattern: Counters.* (dynamically created counters)
            if (key.StartsWith("Counters."))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a label key matches known dynamic patterns.
        /// Allows common Ink patterns that aren't explicitly registered.
        /// </summary>
        private static bool IsValidDynamicLabelKey(string key)
        {
            // Pattern: player.* (any player label)
            if (key.StartsWith("player."))
                return true;

            // Pattern: Labels.* (dynamically created labels)
            if (key.StartsWith("Labels."))
                return true;

            return false;
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
