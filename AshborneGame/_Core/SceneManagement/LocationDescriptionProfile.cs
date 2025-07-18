using System;
using System.Collections.Generic;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Manages all types of descriptive text, including conditionals.
    /// </summary>
    public class LocationDescriptionProfile
    {
        /// <summary>
        /// Description shown the first time the player visits.
        /// </summary>
        public string FirstTimeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Description shown on return visits.
        /// </summary>
        public string ReturnDescription { get; set; } = string.Empty;

        /// <summary>
        /// Description shown when the player looks around.
        /// </summary>
        public string LookAroundDescription { get; set; } = string.Empty;

        /// <summary>
        /// List of ambient snippets for flavor.
        /// </summary>
        public List<string> AmbientSnippets { get; set; } = new();

        /// <summary>
        /// Optional conditional descriptions.
        /// </summary>
        public DescribableBehaviour? ConditionalDescriptions { get; set; }

        private bool _visited = false;

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state)
        {
            string baseDesc = _visited ? ReturnDescription : FirstTimeDescription;
            _visited = true;

            string conditional = ConditionalDescriptions?.GetDescription(player, state) ?? string.Empty;
            return baseDesc + (string.IsNullOrWhiteSpace(conditional) ? "" : " " + conditional);
        }

        /// <summary>
        /// Returns the look around description for the player and state.
        /// </summary>
        public string GetLookAroundDescription(Player player, GameStateManager state)
        {
            return LookAroundDescription + (ConditionalDescriptions?.GetDescription(player, state) ?? string.Empty);
        }

        /// <summary>
        /// Returns a random ambient snippet, or empty string if none.
        /// </summary>
        public string GetRandomAmbientDescription()
        {
            if (AmbientSnippets.Count == 0) return string.Empty;
            return AmbientSnippets[new Random().Next(AmbientSnippets.Count)];
        }
    }
} 