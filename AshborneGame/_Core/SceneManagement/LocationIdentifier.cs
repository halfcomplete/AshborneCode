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
        public List<string> Synonyms { get; }

        /// <summary>
        /// Creates a new LocationDescriptor.
        /// </summary>
        public LocationIdentifier(string referenceName, List<string>? synonyms = null, string article = "the")
        {
            ReferenceName = referenceName;
            Synonyms = synonyms ?? new List<string>();
            Article = article;
        }

        /// <summary>
        /// Checks if the input matches the reference name, display name, or any synonym.
        /// </summary>
        public bool Matches(string input)
        {
            input = input.ToLowerInvariant();
            return input == ReferenceName.ToLowerInvariant() || input == DisplayName || Synonyms.Any(s => s.ToLowerInvariant() == input) || Synonyms.Any(s => (Article + " " + s.ToLowerInvariant()) == input);
        }
    }
} 