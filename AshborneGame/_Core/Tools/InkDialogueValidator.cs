using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.Tools
{
    /// <summary>
    /// Build-time validator for Ink dialogue JSON files.
    /// 
    /// This tool scans all .json dialogue files in the project and validates that the game state
    /// keys referenced within them (setFlag, setCounter, setLabel, getFlag, etc.) match known identifiers
    /// defined in GameStateKeyConstants and EventNameConstants.
    /// 
    /// Run this during build or pre-commit to catch typos in Ink scripts early.
    /// 
    /// Usage:
    ///   InkDialogueValidator.ValidateAllDialogues(rootPath)
    ///   
    /// Example:
    ///   var issues = InkDialogueValidator.ValidateAllDialogues("./AshborneWASM/wwwroot/Dialogue");
    ///   foreach (var issue in issues)
    ///       Console.WriteLine($"[WARNING] {issue.FilePath}:{issue.LineNumber} - {issue.Message}");
    /// </summary>
    public static class InkDialogueValidator
    {
        public record ValidationIssue(
            string FilePath,
            int LineNumber,
            string Message,
            bool IsError
        );

        private static readonly Regex FlagKeyPattern = new(@"""setFlag""|""getFlag""|""hasFlag""|""toggleFlag""|""removeFlag""", RegexOptions.Compiled);
        private static readonly Regex CounterKeyPattern = new(@"""setCounter""|""getCounter""|""hasCounter""|""incCounter""|""decCounter""|""removeCounter""", RegexOptions.Compiled);
        private static readonly Regex LabelKeyPattern = new(@"""setLabel""|""getLabel""|""hasLabel""|""removeLabel""", RegexOptions.Compiled);
        private static readonly Regex StringExtraction = new(@"""([^""\\]|\\.)*""", RegexOptions.Compiled);

        /// <summary>
        /// Validates all .json dialogue files in a directory and subdirectories.
        /// </summary>
        public static List<ValidationIssue> ValidateAllDialogues(string rootPath)
        {
            var issues = new List<ValidationIssue>();

            if (!Directory.Exists(rootPath))
            {
                issues.Add(new ValidationIssue(rootPath, 0, $"Dialogue directory not found: {rootPath}", true));
                return issues;
            }

            var jsonFiles = Directory.GetFiles(rootPath, "*.json", SearchOption.AllDirectories);
            
            foreach (var filePath in jsonFiles)
            {
                try
                {
                    issues.AddRange(ValidateSingleFile(filePath));
                }
                catch (Exception ex)
                {
                    issues.Add(new ValidationIssue(filePath, 0, $"Error reading file: {ex.Message}", true));
                }
            }

            return issues;
        }

        /// <summary>
        /// Validates a single dialogue JSON file for state key consistency.
        /// </summary>
        public static List<ValidationIssue> ValidateSingleFile(string filePath)
        {
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
                issues.Add(new ValidationIssue(filePath, 0, $"Failed to parse file: {ex.Message}", true));
            }

            return issues;
        }

        private static List<(string FunctionName, string Argument)> ExtractExternalFunctionCalls(string json)
        {
            var calls = new List<(string, string)>();

            // Pattern: "x()":"functionName","exArgs":N... followed by argument strings
            var pattern = new Regex(@"""x\(\)"":""(\w+)"",""exArgs"":(\d+)", RegexOptions.Compiled);
            var matches = pattern.Matches(json);

            foreach (Match match in matches)
            {
                var functionName = match.Groups[1].Value;
                var argCount = int.Parse(match.Groups[2].Value);

                // Extract the following N string arguments
                var startPos = match.Index + match.Length;
                for (int i = 0; i < argCount; i++)
                {
                    var argMatch = Regex.Match(json.Substring(startPos), @"""([^""\\]|\\.)*""");
                    if (argMatch.Success)
                    {
                        var argValue = UnquoteString(argMatch.Value);
                        calls.Add((functionName, argValue));
                        startPos += argMatch.Index + argMatch.Length;
                    }
                }
            }

            return calls;
        }

        private static void ValidateFunctionCall(string filePath, (string FunctionName, string Argument) call, List<ValidationIssue> issues)
        {
            var (functionName, argument) = call;

            switch (functionName)
            {
                case "setFlag":
                case "getFlag":
                case "hasFlag":
                case "toggleFlag":
                case "removeFlag":
                    ValidateFlagKey(filePath, argument, issues);
                    break;

                case "setCounter":
                case "getCounter":
                case "hasCounter":
                case "incCounter":
                case "decCounter":
                case "removeCounter":
                    ValidateCounterKey(filePath, argument, issues);
                    break;

                case "setLabel":
                case "getLabel":
                case "hasLabel":
                case "removeLabel":
                    ValidateLabelKey(filePath, argument, issues);
                    break;
            }
        }

        private static void ValidateFlagKey(string filePath, string key, List<ValidationIssue> issues)
        {
            // Check against registered keys
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredFlagKeys();
            if (registeredKeys.Contains(key))
                return;

            // Check against dynamic patterns
            if (IsValidDynamicFlagKey(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                0, // Line number is hard to extract from minified JSON
                $"Unknown flag key in Ink script: '{key}'. Not found in GameStateKeyConstants or whitelisted patterns. " +
                $"Known flags: {string.Join(", ", registeredKeys.Take(5))}... " +
                $"Allowed patterns: player.actions.*, player.received_*, Flags.Player.*",
                false // Warning, not error
            ));
        }

        private static void ValidateCounterKey(string filePath, string key, List<ValidationIssue> issues)
        {
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredCounterKeys();
            if (registeredKeys.Contains(key))
                return;

            if (IsValidDynamicCounterKey(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                0,
                $"Unknown counter key in Ink script: '{key}'. Not found in GameStateKeyConstants or whitelisted patterns. " +
                $"Known counters: {string.Join(", ", registeredKeys.Take(5))}... " +
                $"Allowed patterns: player.*, Counters.*",
                false // Warning, not error
            ));
        }

        private static void ValidateLabelKey(string filePath, string key, List<ValidationIssue> issues)
        {
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredLabelKeys();
            if (registeredKeys.Contains(key))
                return;

            if (IsValidDynamicLabelKey(key))
                return;

            issues.Add(new ValidationIssue(
                filePath,
                0,
                $"Unknown label key in Ink script: '{key}'. Not found in GameStateKeyConstants or whitelisted patterns. " +
                $"Known labels: {string.Join(", ", registeredKeys)}... " +
                $"Allowed patterns: player.*, Labels.*",
                false // Warning, not error
            ));
        }

        private static bool IsValidDynamicFlagKey(string key)
        {
            return key.StartsWith("player.actions.") ||
                   key.StartsWith("player.received_") ||
                   key.StartsWith("Flags.Player.");
        }

        private static bool IsValidDynamicCounterKey(string key)
        {
            return key.StartsWith("player.") ||
                   key.StartsWith("Counters.");
        }

        private static bool IsValidDynamicLabelKey(string key)
        {
            return key.StartsWith("player.") ||
                   key.StartsWith("Labels.");
        }

        private static string UnquoteString(string quoted)
        {
            if (quoted.StartsWith("\"") && quoted.EndsWith("\""))
                return quoted.Substring(1, quoted.Length - 2);
            return quoted;
        }

        /// <summary>
        /// Prints validation results to console in a readable format.
        /// </summary>
        public static void PrintResults(List<ValidationIssue> issues)
        {
            if (issues.Count == 0)
            {
                Console.WriteLine("[✓] All Ink dialogue files validated successfully!");
                return;
            }

            var errors = issues.Where(i => i.IsError).ToList();
            var warnings = issues.Where(i => !i.IsError).ToList();

            if (warnings.Count > 0)
            {
                Console.WriteLine($"\n[⚠] {warnings.Count} warning(s) found:");
                foreach (var issue in warnings)
                {
                    Console.WriteLine($"  {issue.FilePath}:{issue.LineNumber} - {issue.Message}");
                }
            }

            if (errors.Count > 0)
            {
                Console.WriteLine($"\n[✗] {errors.Count} error(s) found:");
                foreach (var issue in errors)
                {
                    Console.WriteLine($"  {issue.FilePath}:{issue.LineNumber} - {issue.Message}");
                }
                throw new InvalidOperationException($"Ink dialogue validation failed with {errors.Count} error(s).");
            }
        }
    }
}
