using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.CommandHandling
{
    public class CustomCommandPhrasing
    {
        private List<string> _verbs;
        private List<string> _nounPhrases;

        public List<string> Phrases { get; }

        public CustomCommandPhrasing(List<string> verbs, List<string> nounPhrases)
        {
            _verbs = verbs;
            _nounPhrases = nounPhrases;

            Phrases = GeneratePhrases();
        }

        private List<string> GeneratePhrases()
        {
            List<string> phrases = new List<string>();
            foreach (var verb in _verbs)
            {
                foreach (var nounPhrase in _nounPhrases)
                {
                    string phrase = $"{verb} {nounPhrase}".Trim();
                    phrases.Add($"{phrase}");
                }
            }
            return phrases;
        }
    }
}
