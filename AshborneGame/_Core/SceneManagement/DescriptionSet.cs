using System;
using System.Collections.Generic;

namespace AshborneGame._Core.Game.Description
{
    public class DescriptionSet
    {
        // Core identifiers
        /// <summary>
        /// Lower-case shorthand identifier
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Lower-case descriptive identifier
        /// </summary>
        public string DescriptiveName { get; set; } = "";

        // General state-based descriptions
        /// <summary>
        /// Description for the first time the player visits this area. Should be shorter than LookAroundDescription and slightly less detailed.
        /// </summary>
        public string FirstVisitDescription { get; set; } = "";
        /// <summary>
        /// Description for any time the player returns to this area. Should be shorter than LookAroundDescription and slightly less detailed.
        /// </summary>
        public string ReturnVisitDescription { get; set; } = "It's the same as before.";
        /// <summary>
        /// Description for the "look" command. Should be longer than LookAroundDescription and slightly more detailed.
        /// </summary>
        public string LookAroundDescription { get; set; } = "";

        // Mood-based variants (based on world tone, mask effects, etc.)
        public string CalmDescription { get; set; } = "";
        public string TenseDescription { get; set; } = "";
        public string OppressiveDescription { get; set; } = "";

        // Short summaries for UI/commands/logs
        public string OneLineSummary { get; set; } = "";

        // Optional dynamic fragments
        public List<string> RandomFlavorSnippets { get; set; } = new();
        public Dictionary<string, string> FlagSpecificDescriptions { get; set; } = new(); // e.g. "player.has_torch" => "..."

        // Utility to get the best description for a given moment
        public string GetArrivalDescription(bool isFirstVisit, GameStateManager gameState)
        {
            // Check first-visit
            if (isFirstVisit && !string.IsNullOrWhiteSpace(FirstVisitDescription))
                return FirstVisitDescription;

            // Flag-specific override
            foreach (var kvp in FlagSpecificDescriptions)
            {
                if (gameState.GetFlag(kvp.Key) == true)
                    return kvp.Value;
            }

            // Default fallback
            return string.IsNullOrWhiteSpace(ReturnVisitDescription)
                ? $"You are in {DescriptiveName}."
                : ReturnVisitDescription;
        }

        // Looks around regardless of visit status
        public string GetLookAroundDescription()
        {
            return string.IsNullOrWhiteSpace(LookAroundDescription)
                ? $"You look around the {Name}. There's nothing special."
                : LookAroundDescription;
        }

        // Add some randomness for immersion
        private string GetRandomFlavor()
        {
            if (RandomFlavorSnippets == null || RandomFlavorSnippets.Count == 0)
                return "";

            var rand = new Random();
            return RandomFlavorSnippets[rand.Next(RandomFlavorSnippets.Count)];
        }

        // Optional: Mood-based override (maybe driven by a mask)
        public string GetMoodDescription(string mood)
        {
            return mood switch
            {
                "calm" => CalmDescription,
                "tense" => TenseDescription,
                "oppressive" => OppressiveDescription,
                _ => ""
            };
        }

        public override string ToString() => DescriptiveName ?? Name ?? base.ToString();
    }
}
