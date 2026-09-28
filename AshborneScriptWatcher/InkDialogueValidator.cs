using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

namespace AshborneTooling
{
    /// <summary>
    /// Build-time validator for Ink dialogue JSON files.
    /// </summary>
    /// <remarks>
    /// This tool scans all .json dialogue files in the project and validates that the game state
    /// keys referenced within them (setFlag, setCounter, setLabel, getFlag, etc.) match known identifiers
    /// defined in StateKeys and EventNameConstants.
    /// 
    /// Run this during build or pre-commit to catch typos in Ink scripts early.
    /// </remarks>
    /// <example>
    /// Usage:
    /// <code>
    ///   InkDialogueValidator.ValidateAllDialogues(rootPath)
    /// </code>
    /// Example:
    /// <code>
    ///   var issues = InkDialogueValidator.ValidateAllDialogues("./AshborneWASM/wwwroot/Dialogue");
    ///   foreach (var issue in issues)
    ///       Console.WriteLine($"[WARNING] {issue.FilePath}:{issue.LineNumber} - {issue.Message}");
    /// </code>
    /// </example>
    public static class InkDialogueValidator
    {
        public record ValidationIssue(
            string FilePath,
            string? FailedLine,
            string Message
        );

        private static readonly HashSet<string> KnownFunctions =
        [
            InkExternalFunctionNames.SetFlag, InkExternalFunctionNames.GetFlag, InkExternalFunctionNames.HasFlag,
            InkExternalFunctionNames.ToggleFlag, InkExternalFunctionNames.RemoveFlag,
            InkExternalFunctionNames.SetCounter, InkExternalFunctionNames.GetCounter, InkExternalFunctionNames.HasCounter,
            InkExternalFunctionNames.IncCounter, InkExternalFunctionNames.DecCounter, InkExternalFunctionNames.RemoveCounter,
            InkExternalFunctionNames.SetLabel, InkExternalFunctionNames.GetLabel, InkExternalFunctionNames.HasLabel,
            InkExternalFunctionNames.RemoveLabel, InkExternalFunctionNames.PlayerHas,
            InkExternalFunctionNames.PlayerForceMask, InkExternalFunctionNames.PlayerGiveMask,
            InkExternalFunctionNames.PlayerTryTakeMask, InkExternalFunctionNames.PlayerWearingMask,
            InkExternalFunctionNames.ChangePlayerStat, InkExternalFunctionNames.GetPlayerStat,
            InkExternalFunctionNames.GetLocationVisits, InkExternalFunctionNames.IncLocationVisits,
            InkExternalFunctionNames.AdvanceTime, InkExternalFunctionNames.AddSyntheticMemory,
            InkExternalFunctionNames.GetNpcEmotion, InkExternalFunctionNames.HasNpcMemory,
            InkExternalFunctionNames.GetNpcMemoryCount, InkExternalFunctionNames.EventBegin,
            InkExternalFunctionNames.EventAddParticipant, InkExternalFunctionNames.EventAddData,
            InkExternalFunctionNames.EventCommit, InkExternalFunctionNames.SetSilentPath,
            InkExternalFunctionNames.AnimateBlur
        ];

        private sealed record ExternalFunctionCall(string Name, string[] Arguments, int CompiledArity, string FullLine);

        /// <summary>
        /// Validates all .json dialogue files in a directory and subdirectories.
        /// </summary>
        public static List<ValidationIssue> ValidateAllDialogues(string rootPath)
        {
            var issues = new List<ValidationIssue>();

            if (!Directory.Exists(rootPath))
            {
                issues.Add(new ValidationIssue(rootPath, null, $"Dialogue directory not found: {rootPath}"));
                return issues;
            }

            // Get all files in the provided root path and its subdirectories with the .json extension
            var jsonFiles = Directory.GetFiles(rootPath, "*.json", SearchOption.AllDirectories);
            
            foreach (var filePath in jsonFiles)
            {
                try
                {
                    issues.AddRange(ValidateSingleFile(filePath));
                }
                catch (Exception ex)
                {
                    issues.Add(new ValidationIssue(filePath, null, $"Error reading file: {ex.Message}"));
                }
            }

            return issues;
        }

        /// <summary>
        /// Validates a single dialogue JSON file for state key consistency.
        /// </summary>
        public static List<ValidationIssue> ValidateSingleFile(string filePath)
        {
            Console.WriteLine("Validating Ink dialogue file: " + filePath);
            var issues = new List<ValidationIssue>();

            try
            {
                string json = File.ReadAllText(filePath);
                
                // Extract all external function calls and their string arguments
                var functionCalls = ExtractExternalFunctionCalls(json);

                foreach (var call in functionCalls)
                {
                    ValidateFunctionCall(filePath, call, issues);
                }
            }
            catch (Exception ex)
            {
                issues.Add(new ValidationIssue(filePath, null, $"Failed to parse file due to error: {ex.InnerException?.Message ?? ex.Message} at {ex.InnerException?.StackTrace ?? ex.StackTrace}"));
            }

            return issues;
        }

        private static List<ExternalFunctionCall> ExtractExternalFunctionCalls(string json)
        {
            var calls = new List<ExternalFunctionCall>();

            var matches = OutputConstants.InkFunctionRegex.Matches(json);

            foreach (Match match in matches)
            {
                var parameters = match.Groups[1].Value;
                var functionName = match.Groups[2].Value;
                int paramCount = int.Parse(match.Groups[3].Value);

                // Extract parameters from the parameters match
                string[] splitParameters = RemoveStringMarkers(parameters.Split(','))
                    .Select(parameter => parameter.Trim())
                    .Where(parameter => parameter.Length > 0)
                    .ToArray();
                calls.Add(new ExternalFunctionCall(
                    functionName,
                    splitParameters,
                    paramCount,
                    $"~ {functionName}({string.Join(", ", splitParameters)})"));
            }

            return calls;
        }

        private static void ValidateFunctionCall(string filePath, ExternalFunctionCall call, List<ValidationIssue> issues)
        {
            string functionName = call.Name;
            if (!KnownFunctions.Contains(functionName))
            {
                issues.Add(new ValidationIssue(filePath, call.FullLine, $"'{functionName}' is not a registered Ink external function."));
                return;
            }

            int expectedArity = GetExpectedArity(functionName);
            if (call.CompiledArity != expectedArity || call.Arguments.Length != expectedArity)
            {
                issues.Add(new ValidationIssue(filePath, call.FullLine,
                    $"'{functionName}' expects {expectedArity} argument(s), but compiled Ink contains {call.CompiledArity}."));
                return;
            }

            string argument = call.Arguments.Length > 0
                ? RemoveInkJSONUpArrow(RemoveQuotes(call.Arguments[0]))
                : string.Empty;

            switch (functionName)
            {
                case InkExternalFunctionNames.SetFlag:
                case InkExternalFunctionNames.GetFlag:
                case InkExternalFunctionNames.HasFlag:
                case InkExternalFunctionNames.ToggleFlag:
                case InkExternalFunctionNames.RemoveFlag:
                    ValidateFlagKey(filePath, argument, issues, call.FullLine);
                    break;

                case InkExternalFunctionNames.SetCounter:
                case InkExternalFunctionNames.GetCounter:
                case InkExternalFunctionNames.HasCounter:
                case InkExternalFunctionNames.IncCounter:
                case InkExternalFunctionNames.DecCounter:
                case InkExternalFunctionNames.RemoveCounter:
                    ValidateCounterKey(filePath, argument, issues, call.FullLine);
                    break;

                case InkExternalFunctionNames.SetLabel:
                case InkExternalFunctionNames.GetLabel:
                case InkExternalFunctionNames.HasLabel:
                case InkExternalFunctionNames.RemoveLabel:
                    ValidateLabelKey(filePath, argument, issues, call.FullLine);
                    break;

                case InkExternalFunctionNames.GetNpcEmotion:
                    ValidateEnumArgument(filePath, call, 1, issues, typeof(EmotionType), "emotion");
                    break;
                case InkExternalFunctionNames.EventAddParticipant:
                    ValidateEnumCsvArgument(filePath, call, 1, issues, typeof(MemoryRole), "memory role");
                    break;
                case InkExternalFunctionNames.AddSyntheticMemory:
                    ValidateEnumCsvArgument(filePath, call, 0, issues, typeof(MemoryTagType), "memory tag");
                    break;
                case InkExternalFunctionNames.HasNpcMemory:
                case InkExternalFunctionNames.GetNpcMemoryCount:
                    ValidateEnumCsvArgument(filePath, call, 1, issues, typeof(MemoryTagType), "memory tag");
                    break;
            }
        }

        private static int GetExpectedArity(string functionName) => functionName switch
        {
            InkExternalFunctionNames.SetFlag or InkExternalFunctionNames.SetCounter or InkExternalFunctionNames.SetLabel => 2,
            InkExternalFunctionNames.IncCounter or InkExternalFunctionNames.DecCounter => 2,
            InkExternalFunctionNames.AddSyntheticMemory or InkExternalFunctionNames.GetNpcEmotion or InkExternalFunctionNames.HasNpcMemory or InkExternalFunctionNames.GetNpcMemoryCount or InkExternalFunctionNames.EventAddParticipant or InkExternalFunctionNames.EventAddData or InkExternalFunctionNames.SetSilentPath => 2,
            InkExternalFunctionNames.ChangePlayerStat => 2,
            InkExternalFunctionNames.AnimateBlur => 4,
            InkExternalFunctionNames.GetFlag or InkExternalFunctionNames.HasFlag or InkExternalFunctionNames.ToggleFlag or InkExternalFunctionNames.RemoveFlag or InkExternalFunctionNames.GetCounter or InkExternalFunctionNames.HasCounter or InkExternalFunctionNames.RemoveCounter or InkExternalFunctionNames.GetLabel or InkExternalFunctionNames.HasLabel or InkExternalFunctionNames.RemoveLabel or InkExternalFunctionNames.PlayerHas or InkExternalFunctionNames.PlayerForceMask or InkExternalFunctionNames.PlayerGiveMask or InkExternalFunctionNames.PlayerTryTakeMask or InkExternalFunctionNames.PlayerWearingMask or InkExternalFunctionNames.GetPlayerStat or InkExternalFunctionNames.GetLocationVisits or InkExternalFunctionNames.IncLocationVisits or InkExternalFunctionNames.EventBegin => 1,
            InkExternalFunctionNames.AdvanceTime or InkExternalFunctionNames.EventCommit => 0,
            _ => throw new InvalidOperationException($"No compile-time arity is registered for '{functionName}'.")
        };

        private static void ValidateEnumArgument(string filePath, ExternalFunctionCall call, int index, List<ValidationIssue> issues, Type enumType, string description)
        {
            string value = RemoveInkJSONUpArrow(RemoveQuotes(call.Arguments[index]));
            if (call.Arguments[index].StartsWith("^", StringComparison.Ordinal) && !Enum.TryParse(enumType, value, true, out _))
                issues.Add(new ValidationIssue(filePath, call.FullLine, $"'{value}' is not a valid {description} for '{call.Name}'."));
        }

        private static void ValidateEnumCsvArgument(string filePath, ExternalFunctionCall call, int index, List<ValidationIssue> issues, Type enumType, string description)
        {
            string rawValue = call.Arguments[index];
            if (!rawValue.StartsWith("^", StringComparison.Ordinal))
                return;

            string value = RemoveInkJSONUpArrow(RemoveQuotes(rawValue));
            foreach (string item in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!Enum.TryParse(enumType, item, true, out _))
                    issues.Add(new ValidationIssue(filePath, call.FullLine, $"'{item}' is not a valid {description} for '{call.Name}'."));
            }
        }

        private static void ValidateFlagKey(string filePath, string key, List<ValidationIssue> issues, string line)
        {
            // Check against registered keys
            var registeredKeys = GameStateTracker.GetAllRegisteredFlagKeys();
            key = "Flags." + key;
            if (registeredKeys.Contains(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                line,
                $"'{key}' is not a registered flag key."
            ));
        }

        private static void ValidateCounterKey(string filePath, string key, List<ValidationIssue> issues, string line)
        {
            var registeredKeys = GameStateTracker.GetAllRegisteredCounterKeys();
            key = "Counters." + key;
            if (registeredKeys.Contains(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                line,
                $"'{key}' is not a registered counter key."
            ));
        }

        private static void ValidateLabelKey(string filePath, string key, List<ValidationIssue> issues, string line)
        {
            var registeredKeys = GameStateTracker.GetAllRegisteredLabelKeys();
            key = "Labels." + key;
            if (registeredKeys.Contains(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                line,
                $"'{key}' is not a registered label key."
            ));
        }

        private static string RemoveQuotes(string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                return input[1..^1];
            }
            return input;
        }

        private static string RemoveInkJSONUpArrow(string input)
        {
            if (input.StartsWith("^"))
            {
                return input[1..];
            }
            return input;
        }

        private static string[] RemoveStringMarkers(string[] input)
        {
            return input.Where(s => !s.Equals("\"str\"") && !s.Equals("\"/str\"")).ToArray();
        }
        /// <summary>
        /// Prints validation results to console in a readable format.
        /// </summary>
        public static void PrintResults(List<ValidationIssue> issues)
        {
            if (issues.Count == 0)
            {
                Console.WriteLine("[SUCCESS] All Ink dialogue files validated successfully!");
                return;
            }
            Console.WriteLine();
            Console.WriteLine($"\n[ERROR] {issues.Count} error(s) found:");
            foreach (var issue in issues)
            {
                Console.WriteLine($"In {issue.FilePath}: {issue.FailedLine} - ({issue.Message})");
            }
            throw new InvalidOperationException($"Ink dialogue validation failed with {issues.Count} error(s).");
        }
    }
}
