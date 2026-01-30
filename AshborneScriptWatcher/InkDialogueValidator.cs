using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;
using AshborneGame._Core.Globals.Constants;

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

        private static List<(string, string, string)> ExtractExternalFunctionCalls(string json)
        {
            var calls = new List<(string, string, string)>();

            var matches = OutputConstants.InkFunctionRegex.Matches(json);

            foreach (Match match in matches)
            {
                var parameters = match.Groups[1].Value;
                var functionName = match.Groups[2].Value;
                var paramCount = int.Parse(match.Groups[3].Value);

                // Extract parameters from the parameters match
                var splitParameters = RemoveStringMarkers(parameters.Split(','));

                foreach (var p in splitParameters)
                {
                    switch (functionName)
                    {
                        case "setFlag":
                        case "setCounter":
                        case "setLabel":
                            //calls.Add((functionName, RemoveQuotes(splitParameters[1]), $"~ {functionName}({string.Join(", ", splitParameters)})"));
                            //break;

                        case "hasFlag":
                        case "getFlag":
                        case "toggleFlag":
                        case "removeFlag":
                        case "hasCounter":
                        case "incCounter":
                        case "decCounter":
                        case "getCounter":
                        case "removeCounter":
                        case "hasLabel":
                        case "getLabel":
                        case "removeLabel":
                            calls.Add((functionName, RemoveInkJSONUpArrow(RemoveQuotes(splitParameters[0])), $"~ {functionName}{string.Join(", ", splitParameters)}"));
                            break;
                    }
                }
            }

            return calls;
        }

        private static void ValidateFunctionCall(string filePath, (string, string, string) call, List<ValidationIssue> issues)
        {
            var (functionName, argument, fullLine) = call;

            switch (functionName)
            {
                case "setFlag":
                case "getFlag":
                case "hasFlag":
                case "toggleFlag":
                case "removeFlag":
                    ValidateFlagKey(filePath, argument, issues, fullLine);
                    break;

                case "setCounter":
                case "getCounter":
                case "hasCounter":
                case "incCounter":
                case "decCounter":
                case "removeCounter":
                    ValidateCounterKey(filePath, argument, issues, fullLine);
                    break;

                case "setLabel":
                case "getLabel":
                case "hasLabel":
                case "removeLabel":
                    ValidateLabelKey(filePath, argument, issues, fullLine);
                    break;
            }
        }

        private static void ValidateFlagKey(string filePath, string key, List<ValidationIssue> issues, string line)
        {
            // Check against registered keys
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredFlagKeys();
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
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredCounterKeys();
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
            var registeredKeys = InkStateKeyRegistry.GetAllRegisteredLabelKeys();
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
