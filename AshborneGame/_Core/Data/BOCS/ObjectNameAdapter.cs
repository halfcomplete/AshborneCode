using AshborneGame._Core.Globals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS
{
    public class ObjectNameAdapter
    {
        public string ReferenceName { get; }
        public string FirstTimeDisplayName { get; }

        public bool Seen { get; set; } = false;

        private string? _article;

        public string? Article
        {
            get
            {
                return _article;
            }
            set
            {
                _article = value;
            }
        }

        public string DisplayName
        {
            get
            {
                if (Article == null) return ReferenceName;
                else
                {
                    if (Seen) return Article + " " + ReferenceName;
                    else
                        return FirstTimeDisplayName;
                }
            }
        }

        public List<string> Synonyms { get; private set; }

        public ObjectNameAdapter(string reference, string firstTimeDisplayName, List<string> synonyms, string? article = "the")
        {
            ReferenceName = reference;
            FirstTimeDisplayName = firstTimeDisplayName;
            Article = article;
            Synonyms = synonyms;
        }

        /// <summary>
        /// Checks if the input matches the reference name, display name, or any synonym.
        /// </summary>
        public bool Matches(string input)
        {
            return MatchesDisplayName(input) || MatchesReferenceNameOrSynonyms(input) || MatchesDisplayNameWithSynonyms(input);
        }

        public bool DoesNotMatch(string input)
        {
            return !Matches(input);
        }

        public bool MatchesReferenceName(string input) => input.ToLowerInvariant() == ReferenceName.ToLowerInvariant();

        public bool MatchesDisplayName(string input) => input.ToLowerInvariant() == DisplayName.ToLowerInvariant();

        public bool MatchesReferenceNameOrSynonyms(string input) => MatchesReferenceName(input) || Synonyms.Any(s => input.ToLowerInvariant() == s.ToLowerInvariant());

        public bool MatchesDisplayNameWithSynonyms(string input) => Synonyms.Any(s => input.ToLowerInvariant() == Article + " " + s.ToLowerInvariant());

        public override string ToString()
        {
            return ReferenceName;
        }

        public static implicit operator string(ObjectNameAdapter name)
        {
            return name.ReferenceName;
        }

        public ObjectNameAdapter DeepClone()
        {
            return new ObjectNameAdapter(ReferenceName, FirstTimeDisplayName, new(Synonyms), Article);
        }
    }
}
