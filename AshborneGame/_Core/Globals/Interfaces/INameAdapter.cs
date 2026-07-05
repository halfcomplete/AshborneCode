using AshborneGame._Core.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface INameAdapter
    {
        /// <summary>
        /// The core name of the location (e.g., "Dusty Armoury").
        /// </summary>
        public string ReferenceName { get; }

        /// <summary>
        /// Article to use (e.g., "the").
        /// </summary>
        public string Article { get; set; }


        /// <summary>
        /// Display name (e.g., "the Dusty Armoury").
        /// </summary>
        public string DisplayName => $"{Article} {ReferenceName}";

        /// <summary>
        /// List of synonyms (substitutes for the ReferenceName) for parsing and matching.
        /// </summary>
        public List<string> Synonyms { get; }

        /// <summary>
        /// Checks if the input matches the reference name, display name, or any synonym.
        /// </summary>
        public bool Matches(string input)
        {
            return MatchesDisplayName(input) || MatchesReferenceNameOrSynonyms(input) || MatchesDisplayNameWithSynonyms(input);
        }

        public bool MatchesReferenceName(string input) => input.ToLowerInvariant() == ReferenceName.ToLowerInvariant();

        public bool MatchesDisplayName(string input) => input.ToLowerInvariant() == DisplayName.ToLowerInvariant();

        public bool MatchesReferenceNameOrSynonyms(string input) => MatchesReferenceName(input) || Synonyms.Any(s => input.ToLowerInvariant() == s.ToLowerInvariant());

        public bool MatchesDisplayNameWithSynonyms(string input) => Synonyms.Any(s => input.ToLowerInvariant() == Article + " " + s.ToLowerInvariant());
    }
}
