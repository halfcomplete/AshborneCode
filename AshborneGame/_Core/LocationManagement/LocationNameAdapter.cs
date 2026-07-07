using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.LocationManagement
{
    /// <summary>
    /// Supports flexible naming for parsing, immersion, and varied prose.
    /// </summary>
    public class LocationNameAdapter
    {
        /// <summary>
        /// The core name of the location (e.g., "Dusty Armoury").
        /// </summary>
        public string ReferenceName { get; }

        public string FirstTimeDisplayName { get; }

        /// <summary>
        /// Article to use (e.g., "the").
        /// </summary>
        private string? _article;

        /// <summary>
        /// Article to use (e.g., "the").
        /// </summary>
        public string? Article
        {
            get
            {
                if (_parentLocation != null)
                {
                    if (_parentLocation.VisitCount == 0)
                        return "a";

                    if (_parentLocation.VisitCount == 1)
                        return _article;
                }

                // default
                return _article;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Article cannot be null or whitespace.", nameof(value));
                }

                _article = value;
            }
        }


        /// <summary>
        /// Display name (e.g., "the Dusty Armoury").
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Article == null) return ReferenceName;
                else
                {
                    if (_parentLocation != null && _parentLocation.VisitCount == 0)
                    {
                        return FirstTimeDisplayName;
                    }
                    return Article + " " + ReferenceName;
                }
            }
        }

        /// <summary>
        /// List of synonyms for parsing and matching.
        /// </summary>
        public List<string> Synonyms { get; }

        private Location _parentLocation;

        /// <summary>
        /// Creates a new LocationNameAdapter.
        /// </summary>
        public LocationNameAdapter(string referenceName, string firstTimeDisplayName, List<string>? synonyms = null, string article = "the")
        {
            ReferenceName = referenceName;
            FirstTimeDisplayName = firstTimeDisplayName;
            Synonyms = synonyms ?? new List<string>();
            Article = article;
        }

        public void SetParentLocation(Location location)
        {
            if (_parentLocation != null)
                throw new InvalidOperationException("Parent location is already set.");
            _parentLocation = location ?? throw new ArgumentNullException(nameof(location), "Parent location cannot be null.");
        }

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