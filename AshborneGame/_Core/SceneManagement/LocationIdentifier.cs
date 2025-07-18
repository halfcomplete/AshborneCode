using System.Collections.Generic;
using System.Linq;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Supports flexible naming for parsing, immersion, and varied prose.
    /// </summary>
    public class LocationIdentifier
    {
        /// <summary>
        /// The core name of the location (e.g., "Dusty Armoury").
        /// </summary>
        public string ReferenceName { get; }

        /// <summary>
        /// Article to use (e.g., "the").
        /// </summary>
        public string Article { get; } = "the";

        /// <summary>
        /// Display name (e.g., "the Dusty Armoury").
        /// </summary>
        public string DisplayName => $"{Article} {ReferenceName}";

        /// <summary>
        /// List of synonyms for parsing and matching.
        /// </summary>
        public List<string> Synonyms { get; } = new();

        /// <summary>
        /// Creates a new LocationDescriptor.
        /// </summary>
        public LocationIdentifier(string referenceName, string article = "the")
        {
            ReferenceName = referenceName;
            Article = article;
        }

        /// <summary>
        /// Checks if the input matches the reference name or any synonym.
        /// </summary>
        public bool Matches(string input)
        {
            input = input.ToLowerInvariant();
            return input == ReferenceName.ToLowerInvariant() || Synonyms.Any(s => s.ToLowerInvariant() == input);
        }
    }
} 